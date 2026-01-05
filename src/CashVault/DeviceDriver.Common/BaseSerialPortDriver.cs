using System.IO.Ports;
using Microsoft.Extensions.Logging;

namespace CashVault.DeviceDriver.Common;

/// <summary>
/// Represents a serial port driver for the bill dispenser.
/// </summary>
public abstract class BaseSerialPortDriver : ISerialPortDriver
{
    private bool disposedValue;
    private SerialPort? serialPort;
    protected bool IsSerialPortConnected => serialPort?.IsOpen ?? false;

    private readonly SemaphoreSlim readWriteSemaphore = new(1);
    private readonly SemaphoreSlim sendingMessageSemaphore = new(1);

    protected SerialPortConfiguration? portConfiguration;

    private CancellationTokenSource? cancellationTokenForProcessingThread;

    protected ILogger logger;
    protected LocalDevEnvOptions? LocalDevEnvOptions { get; private set; }

    public event EventHandler DeviceDisconnected;

    public BaseSerialPortDriver(ILogger logger, LocalDevEnvOptions? localDevEnvOptions = null)
    {
        this.logger = logger;
        LocalDevEnvOptions = localDevEnvOptions;
    }

    /// <summary>
    /// Closes the connection to the serial port.
    /// </summary>
    /// <returns>True if the connection is successfully closed, otherwise false.</returns>
    public virtual Task<bool> CloseConnectionAsync()
    {
        readWriteSemaphore.Wait();

        try
        {
            if (this.serialPort?.IsOpen == true)
            {
                this.serialPort?.DiscardInBuffer();
                this.serialPort?.DiscardOutBuffer();
                this.serialPort.Close();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while closing the connection to the serial port.");
            throw;
        }
        finally
        {
            readWriteSemaphore.Release();
        }

        if (this.serialPort?.IsOpen == false)
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    /// <summary>
    /// Opens a connection to the serial port with the specified configuration.
    /// </summary>
    /// <param name="portConfiguration">The configuration for the serial port.</param>
    /// <returns>True if the connection is successfully opened, otherwise false.</returns>
    public virtual Task<bool> OpenConnectionAsync(SerialPortConfiguration portConfiguration)
    {
        if (portConfiguration == null)
        {
            logger.LogError("Port configuration is null.");
            throw new ArgumentNullException(nameof(portConfiguration));
        }

        // Only validate ports on non-Unix systems (Windows)
        bool isUnix = Environment.OSVersion.Platform == PlatformID.Unix;
        
        if (!isUnix && !SerialPort.GetPortNames().Contains(portConfiguration.PortName))
        {
            logger.LogError("Specified port name is not available.");
            throw new ArgumentException("Specified port name is not available.");
        }

        // Create a new instance of SerialPort
        this.serialPort = new SerialPort(portConfiguration.PortName);

        // Set the necessary properties of the SerialPort
        this.serialPort.BaudRate = portConfiguration.BaudRate;
        this.serialPort.Parity = portConfiguration.Parity;
        this.serialPort.DataBits = portConfiguration.DataBits;
        this.serialPort.StopBits = portConfiguration.StopBits;
        if (portConfiguration.ReadTimeout > 0)
        {
            this.serialPort.ReadTimeout = portConfiguration.ReadTimeout;
        }
        if (portConfiguration.WriteTimeout > 0)
        {
            this.serialPort.WriteTimeout = portConfiguration.WriteTimeout;
        }
        //serialPort.Handshake = Handshake.RequestToSend;

        this.portConfiguration = portConfiguration;

        try
        {
            // Open the connection to the SerialPort
            this.serialPort.Open();
            this.serialPort.DiscardInBuffer();
            this.serialPort.DiscardOutBuffer();

            this.cancellationTokenForProcessingThread = new CancellationTokenSource();

            if (IsSerialPortConnected)
            {
                return Task.FromResult(true);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while opening the connection to the serial port.");
            this.serialPort.Dispose();
            // TODO: Add logging
        }

        return Task.FromResult(false);
    }

    /// <summary>
    /// Sends an ISerialPortMessage through the serial port. Override this method to implement the specific message sending logic.
    /// </summary>
    /// <param name="message">Instance of ISerialPortMessage</param>
    /// <returns></returns>
    protected virtual bool SendMessageInternal(ISerialPortMessage message)
    {
        return SendMessageInternal(message.GetMessageBytes());
    }

    /// <summary>
    /// Sends a message bytes through the serial port. Override this method to implement the specific message sending logic.
    /// </summary>
    /// <param name="message">Message bytes.</param>
    /// <returns></returns>
    protected virtual bool SendMessageInternal(byte[] message)
    {
        logger.LogInformation("Sending message through the serial port.");
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sends a message through the serial port. Override this method to implement the specific reading sending logic.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected virtual ISerialPortMessage ReadMessageInternal()
    {
        logger.LogInformation("Reading message from the serial port.");
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes the message bytes to the serial port.
    /// </summary>
    /// <param name="messageBytes">Byte array.</param>
    /// <returns> True if write succeded, False if not</returns>
    /// <exception cref="InvalidOperationException">If serial port is not opened.</exception>
    public virtual bool WriteMessageBytesToSerialPort(Span<byte> messageBytes)
    {
        if (IsSerialPortConnected == false)
        {
            logger.LogError("Serial port is not initialized.");
            throw new InvalidOperationException("Serial port is not initialized.");
        }

        try
        {
            readWriteSemaphore.Wait();
            serialPort.Write(messageBytes.ToArray(), 0, messageBytes.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while writing message bytes to the serial port.");
            return false;
        }
        finally
        {
            readWriteSemaphore.Release();
        }

        return true;
    }

    /// <summary>
    /// Method used for reading bytes from the serial port in controlled manner.
    /// </summary>
    /// <param name="length">The target number of bytes to read. By default is 2, but if value == -1, all bytes from read buffer will be read.</param>
    /// <param name="timeoutMilliseconds">Maximum time to wait for the bytes to be read.</param>
    /// <returns></returns>
    /// TODO: Check timeoutMilliseconds it should be propagated from SendAndReceiveMessageAsync -> ReadMessageInternal
    public virtual byte[] ReadSerialPortMessageBytes(int length = 2, int timeoutMilliseconds = 100000)
    {
        if (IsSerialPortConnected == false)
        {
            logger.LogError("Serial port is not initialized.");
            throw new InvalidOperationException("Serial port is not initialized.");
        }

        byte[] responseBuffer = Array.Empty<byte>();

        if (length > 0)
        {
            responseBuffer = new byte[length];
        }

        try
        {
            readWriteSemaphore.Wait();

            // Wait for the response with the specified timeout
            DateTime startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < TimeSpan.FromMilliseconds(timeoutMilliseconds))
            {
                if (serialPort.BytesToRead > 0)
                {
                    if (length == -1)
                    {
                        int bufferLength = serialPort.BytesToRead;
                        responseBuffer = new byte[bufferLength];
                        serialPort.Read(responseBuffer, 0, bufferLength);
                        break;
                    }
                    else if (serialPort.BytesToRead < length)
                    {
                        // Wait for the remaining bytes to be received
                        continue;
                    }

                    responseBuffer = new byte[length];
                    serialPort.Read(responseBuffer, 0, length);
                    break;
                }
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while reading message bytes from the serial port.");
            throw;
        }
        finally
        {
            readWriteSemaphore.Release();
        }

        // Check for reaching timeout without receiving a response
        return responseBuffer.Length > 0 ? responseBuffer : null;
    }


    /// <summary>
    /// Reopens the connection to the serial port with the specified configuration.
    /// </summary>
    /// <param name="newPortConfiguration">The new configuration for the serial port.</param>
    /// <returns>True if the connection is successfully reopened, otherwise false.</returns>
    public virtual Task<bool> ReopenConnectionAsync(SerialPortConfiguration newPortConfiguration)
    {
        this.Dispose();

        if (newPortConfiguration == null)
        {
            if (serialPort != null && portConfiguration != null)
            {
                return this.OpenConnectionAsync(portConfiguration);
            }
            else
            {
                logger.LogError("New port configuration is null.");
                // TODO: add logging
                return Task.FromResult(false);
            }
        }
        else
        {
            return this.OpenConnectionAsync(newPortConfiguration);
        }
    }

    public async Task<ISerialPortMessage?> SendAndReceiveMessageAsync(ISerialPortMessage message, long waitingForResponseTimeoutMilliseconds = 2000)
    {
        ISerialPortMessage response = null;

        try
        {
            sendingMessageSemaphore.Wait();

            var bytes = message.GetMessageBytes();
            if (SendMessageInternal(bytes) == false)
            {
                logger.LogError("Error occurred while sending message through the serial port.");
                // TODO: add logging
            }

            DateTime startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < TimeSpan.FromMilliseconds(waitingForResponseTimeoutMilliseconds))
            {
                // Wait for the response
                response = ReadMessageInternal();
                if (response != null)
                {
                    break;
                }
            }
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Operation canceled while sending and receiving message through the serial port.");
            this.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while sending and receiving message through the serial port.");
            throw;
        }
        finally
        {
            sendingMessageSemaphore.Release();
        }

        return response;
    }

    public async Task SendMessageAsync(ISerialPortMessage message)
    {
        if (IsSerialPortConnected == false)
        {
            logger.LogError("Serial port is not initialized.");
            // TODO: add logging
            return;
        }

        try
        {
            sendingMessageSemaphore.Wait();

            var data = message.GetMessageBytes();

            if (SendMessageInternal(data) == false)
            {
                logger.LogError("Error occurred while sending message through the serial port.");
                // TODO: add logging
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while sending message through the serial port.");
            // TODO: add logging
        }
        finally
        {
            sendingMessageSemaphore.Release();
        }
    }

    /// <summary>
    /// Clears both input and output buffers of the serial port.
    /// This should be called before reading to prevent stale data.
    /// </summary>
    protected void ClearSerialPortBuffers()
    {
        try
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
            }
        }
        catch (Exception ex)
        {
            logger?.LogWarning($"Failed to clear serial port buffers: {ex.Message}");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                if (serialPort != null)
                {
                    this.cancellationTokenForProcessingThread?.Cancel();
                    this.CloseConnectionAsync().Wait();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    /// <summary>
    /// Disposes of the resources used by the serial port driver.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public virtual void DeviceReadinessCheck(int maximumRetryCount = 5, long maximumWaitingTime = 10000)
    {
        logger.LogInformation("Performing device readiness check.");
        throw new NotImplementedException();
    }

    public virtual ISerialPortMessage TryParseMessage(byte[] messageBytes)
    {
        logger.LogInformation("Parsing message from the serial port.");
        throw new NotImplementedException();
    }
}
