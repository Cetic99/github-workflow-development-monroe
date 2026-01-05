using CashVault.ccTalk.ccTalkBase.Checksumms;
using CashVault.ccTalk.ccTalkBase.Devices;
using CashVault.ccTalk.ccTalkBase.Messages;
using CashVault.DeviceDriver.Common;
using CashVault.Domain.Aggregates.DeviceAggregate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO.Ports;

namespace CashVault.ccTalk.ccTalkBase;

public abstract partial class BaseCctalkDevice : BaseSerialPortDriver
{
    const Int32 RespondStartTimeout = 2000;
    const Int32 RespondDataTimeout = 50;   //time to wait next byte within message packet recive operation. Correspinds to 11.1 paragraph of ccTalk Generic Specification
                                           // const Int32 RespondDataTimeout = 1500;

    /// <summary>
    /// Semaphore to ensure thread-safe communication
    /// </summary>
    private readonly SemaphoreSlim _communicationLock = new SemaphoreSlim(1, 1);

    private readonly IServiceProvider serviceProvider;

    private ILogger logger => serviceProvider.GetRequiredService<ILogger<BaseCctalkDevice>>();
    private bool _removeEcho = false;
    protected bool RemoveEcho
    {
        get { return _removeEcho; }
        set { _removeEcho = value; }
    }

    public BaseCctalkDevice(Port port, IServiceProvider serviceProvider)
        : base(serviceProvider.GetRequiredService<ILogger<BaseCctalkDevice>>())
    {
        this.serviceProvider = serviceProvider;
        if (port.PortType != PortType.Serial)
        {
            throw new ArgumentException("ccTalkBaseDriver only supports serial ports.", nameof(port));
        }

        this.portConfiguration = new()
        {
            PortName = port.Name,
            BaudRate = 9600,
            DataBits = 8,
            Parity = Parity.Even,
            StopBits = StopBits.One,
            WriteTimeout = 2000,
            ReadTimeout = 2000,
        };
    }

    protected override bool SendMessageInternal(byte[] message)
    {
        try
        {
            this.WriteMessageBytesToSerialPort(message);
        }
        catch (Exception ex)
        {
            Task.Delay(500);
            return false;
        }
        return true;
    }

    protected override ISerialPortMessage ReadMessageInternal()
    {
        byte[] response = null;
        try
        {
            response = this.ReadSerialPortMessageBytes(-1, RespondDataTimeout);
            if (response == null || response.Length == 0)
            {
                logger.LogWarning("Received empty response from device.");
                return null;
            }
            else
            {
                return new CctalkMessage() { Data = response };
            }
        }
        catch (TimeoutException ex)
        {
            logger.LogError("Timeout while reading message from device. Message: {Message}", ex.Message);
            //throw new TimeoutException("Pause in reply (should reset all communication vatiables and be ready to recive the next message)", ex);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError("Error while reading message from device. Message: {Message}", ex.Message);
            return null;
        }
    }

    // TODO: Check async implementation
    //protected async Task<CctalkMessage> CcTalkMsgSend(CctalkMessage com, ICctalkChecksum chHandler)
    protected CctalkMessage CcTalkMsgSend(CctalkMessage com, ICctalkChecksum chHandler)
    {
        _communicationLock.Wait();
        try
        {
            // Clear buffers before starting new communication
            // This prevents stale data from previous commands causing checksum errors
            ClearSerialPortBuffers();

            // TODO: handle BUSY message

            var msgBytes = com.GetTransferDataNoChecksumm();
            chHandler.CalcAndApply(msgBytes);

            CctalkMessage msgReq = new CctalkMessage
            {
                Data = msgBytes
            };

            //await SendMessageAsync(msgReq);
            SendMessageAsync(msgReq).GetAwaiter().GetResult(); // wait for send to complete

            Int32 respondBufPos = 0;
            CctalkMessage respond;
            Byte[] _respondBuf = new byte[255];

            var readTimeout = RespondStartTimeout;

            var echoRemover = 0;
            while (true)
            {
                try
                {
                    var b = ReadSerialPortMessageBytes(1, readTimeout);
                    readTimeout = RespondDataTimeout; // after first byte we expect next bytes within RespondDataTimeout (50ms)

                    if (_removeEcho && (echoRemover < msgBytes.Length))
                    {
                        echoRemover++;
                        continue;
                    }
                    _respondBuf[respondBufPos] = b[0];
                    respondBufPos++;

                    var isRespondComplete = IsRespondComplete(_respondBuf, respondBufPos);
                    if (isRespondComplete)
                    {
                        if (!chHandler.Check(_respondBuf, 0, respondBufPos))
                        {
                            var copy = new byte[respondBufPos];
                            Array.Copy(_respondBuf, copy, respondBufPos);
                            throw new InvalidRespondFormatException(copy, "Checksumm check fail");
                        }
                        respond = ParseRespond(_respondBuf, 0, respondBufPos);
                        Array.Clear(_respondBuf, 0, _respondBuf.Length);
                        break;
                    }

                }
                catch (TimeoutException ex)
                {
                    logger.LogError("Timeout while waiting for respond from device. Message: {Message}", ex.Message);
                    //throw new TimeoutException("Pause in reply (should reset all communication vatiables and be ready to recive the next message)", ex);
                    return null;
                }
                catch (InvalidRespondFormatException ex)
                {
                    logger.LogError("Invalid respond format. Message: {Message}", ex.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError("Error while reading message from ccTalk device. Message: {Message}", ex.Message);
                    return null;
                }
            }

            return respond;

            // IMPORTANT: TODO: CHECK THIS
            /*
             * When receiving bytes within a message packet, the communication software should 
             * wait up to 50ms for another byte if it is expected. If a timeout condition occurs, the 
             * software should reset all communication variables and be ready to receive the next 
             * message. No other action should be taken. (cctalk spec part1, 11.1) 
             */
        }
        finally
        {
            _communicationLock.Release();
        }
    }

    /// <summary>
    /// Disposes resources including the communication lock semaphore
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _communicationLock?.Dispose();
        }
        base.Dispose(disposing);
    }
}

