using System.IO.Ports;
using CashVault.Application.Features.CreditFeatures.Commands;
using CashVault.Application.Features.TicketFeatures.Commands;
using CashVault.BillAcceptorDriver.ID003.Commands.DeviceCommands;
using CashVault.BillAcceptorDriver.ID003.Commands.OperationCommands;
using CashVault.BillAcceptorDriver.ID003.Config;
using CashVault.BillAcceptorDriver.ID003.Interfaces;
using CashVault.BillAcceptorDriver.ID003.Responses;
using CashVault.BillAcceptorDriver.ID003.Responses.CommandResponses;
using CashVault.BillAcceptorDriver.ID003.Responses.ErrorStatusResponses;
using CashVault.BillAcceptorDriver.ID003.Responses.WorkingStatusResponses;
using CashVault.DeviceDriver.Common;
using CashVault.DeviceDriver.Common.Helpers;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CashVault.BillAcceptorDriver.ID003;

public class BillAcceptorDriver : BaseSerialPortDriver, IBillAcceptor
{
    private readonly IServiceProvider serviceProvider;

    private BillAcceptorID003Configuration billAcceptorConfiguration;
    private Thread billAcceptorStatusPoolingThread;
    private CancellationTokenSource cancellationTokenSource;

    private bool isAcceptorInitialized = false;
    private bool isAcceptorEnabled = false;
    private AcceptorStatus currentStatus = AcceptorStatus.Unknown;
    private AcceptingMedia? currentAcceptingMedia;

    // maximum number of consecutive null responses before considering the device disconnected or unresponsive (Unknown status)
    private const int maxNullResponseCount = 5; // (200ms + 200ms)
    private int nullResponseCnt = 0; //helper for Unknown status handling

    public TerminalOperatingMode Mode { get; private set; }

    volatile string deviceWarning = "";
    volatile string deviceError = "";

    public void SetOperatingMode(TerminalOperatingMode mode)
    {
        Mode = mode;
    }

    public async Task<string> GetCurrentStatus()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            return "LocalDevEnv - ReadyForAccepting";
        }
        return Enum.GetName(typeof(AcceptorStatus), currentStatus);
    }

    public string GetWarning()
    {
        return this.deviceWarning;
    }

    public string GetError()
    {
        return this.deviceError;
    }

    public bool IsInitialized => this.isAcceptorInitialized;

    public bool IsConnected
    {
        get
        {
            if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
            {
                return true;
            }
            return this.IsSerialPortConnected;
        }
    }
    public bool IsActive => IsConnected && this.isAcceptorEnabled;

    public bool IsStackBoxRemoved = false;

    public string Name => "CashVault.BillAcceptorDriver.ID003";

    public bool IsEnabled => isAcceptorEnabled;

    public bool CommandInProgress { get; private set; } = false;

    private const string RESET_COMMAND = "Reset";
    private const string ENABLE_COMMAND = "Enable";
    private const string DISABLE_COMMAND = "Disable";

    public IEnumerable<DeviceDiagnosticsCommand> SupportedDiagnosticCommands =>
        [
                new DeviceDiagnosticsCommand(RESET_COMMAND, "Reset"),
                new DeviceDiagnosticsCommand(ENABLE_COMMAND, "Enable"),
                new DeviceDiagnosticsCommand(DISABLE_COMMAND, "Disable"),
        ];

    public event EventHandler DeviceEnabled;
    public event EventHandler DeviceDisabled;
    public event EventHandler<string> ErrorOccured;
    public event EventHandler DeviceDisconnected;
    public event EventHandler<string> WarningOccured;

    public event EventHandler BillAcceptingStarted;
    public event EventHandler<string> BillRejected;
    public event EventHandler<decimal> BillAccepted;
    public event EventHandler StackBoxRemoved;

    public event EventHandler StackBoxFull;
    public event EventHandler JamInAcceptor;
    public event EventHandler JamInStacker;

    public event EventHandler<string> TicketAccepted;
    public event EventHandler<string> TicketRejected;
    public event EventHandler<string> WarningRaised;

    //Sync (1 byte) + PackageLength (1 byte) + Checksum (2 byte)
    private readonly int packageFramingSize = 4;
    private readonly int checksumLength = 2;

    private readonly byte syncByte = 0xFC;

    public BillAcceptorDriver(Port port, BillAcceptorID003Configuration billAcceptorConfiguration, IServiceProvider serviceProvider, LocalDevEnvOptions? localDevEnvOptions = null)
        : base(serviceProvider.GetRequiredService<ILogger<BillAcceptorDriver>>(), localDevEnvOptions)
    {
        this.serviceProvider = serviceProvider;

        if (port.PortType != PortType.Serial)
        {
            throw new ArgumentException("Invalid port type for the bill dispenser driver.");
        }

        this.portConfiguration = new()
        {
            PortName = port.SystemPortName,
            BaudRate = 9600,
            DataBits = 8,
            Parity = Parity.Even,
            StopBits = StopBits.One,
            WriteTimeout = 2000,
            ReadTimeout = 2000,
        };

        this.billAcceptorConfiguration = billAcceptorConfiguration;
        this.cancellationTokenSource = new CancellationTokenSource();
        this.billAcceptorStatusPoolingThread = new Thread(() => PoolingDeviceForStatusResponses(cancellationTokenSource));
        this.WarningOccured += OnWarningOccured;

        var terminal = serviceProvider.GetRequiredService<ITerminal>();
        this.SetOperatingMode(terminal.OperatingMode);

        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            isAcceptorInitialized = true;
            isAcceptorEnabled = true;
            currentStatus = AcceptorStatus.ReadyForAccepting;
        }
    }

    private void OnWarningOccured(object sender, string message)
    {
        logger.LogWarning($"Warning: {message}");
    }

    private async void HandleAcceptorStatusChange(AcceptorStatus newStatus, IAcceptorResponse? response = null)
    {
        var previousStatus = this.currentStatus;
        this.currentStatus = newStatus;

        if (newStatus == previousStatus)
        {
            return;
        }

        string newStatusName = Enum.GetName(typeof(AcceptorStatus), newStatus);
        logger.LogInformation($"Status change: {newStatusName} {response?.GetType()?.ToString()}");

        if (newStatus == AcceptorStatus.Rejecting && currentAcceptingMedia == AcceptingMedia.Bill)
        {
            logger.LogInformation($"Error details: {response?.ToString()}");

            currentAcceptingMedia = null;
            this.BillRejected?.Invoke(this, response?.ToString());
        }
        if (newStatus == AcceptorStatus.Rejecting && currentAcceptingMedia == AcceptingMedia.Ticket)
        {
            logger.LogInformation($"Error details: {response?.ToString()}");

            currentAcceptingMedia = null;
            this.TicketRejected?.Invoke(this, response?.ToString());
        }
        else if (IsStackBoxRemoved && newStatus == AcceptorStatus.Initializing)
        {
            await this.EnableAsync();
            IsStackBoxRemoved = false;
        }
        else if (newStatus == AcceptorStatus.StackerOpened)
        {
            isAcceptorEnabled = false;
            await DisableAsync();
            this.StackBoxRemoved?.Invoke(this, null);
            this.DeviceDisabled?.Invoke(this, null);
            IsStackBoxRemoved = true;
            deviceError = "Stacker box removed.";
        }
        else if (newStatus == AcceptorStatus.Error)
        {
            if (response is StackerFullResponse)
            {
                deviceError = "Stacker box full.";
                logger.LogError($"{deviceError}");
                this.StackBoxFull?.Invoke(this, null);
            }
            else if (response is JamInAcceptorResponse)
            {
                deviceError = "Jam in acceptor.";
                logger.LogError($"{deviceError}");
                this.JamInAcceptor?.Invoke(this, null);
            }
            else if (response is JamInStackerResponse)
            {
                deviceError = "Jam in stacker.";
                logger.LogError($"{deviceError}");
                this.JamInStacker?.Invoke(this, null);
            }
            else if (response is IErrorStatusResponse errorResponse)
            {
                deviceError = errorResponse.ToString();
                logger.LogError($"Error details: {errorResponse.ToString()}");
                this.ErrorOccured?.Invoke(this, errorResponse.ToString());
            }

            this.DeviceDisabled?.Invoke(this, null);
        }
        else if (newStatus == AcceptorStatus.ReadyForAccepting)
        {
            deviceError = "";
            deviceWarning = "";
            this.DeviceEnabled?.Invoke(this, null);
        }
        else if (newStatus == AcceptorStatus.Stacked)
        {
            logger.LogInformation("Bill / coupon stacked.");
        }
        else if (newStatus == AcceptorStatus.Disabled)
        {
            deviceWarning = "Device is disabled.";
            logger.LogInformation($"{deviceWarning}");
            this.DeviceEnabled?.Invoke(this, null);
        }
        else if (newStatus == AcceptorStatus.Unknown)
        {
            if (!this.IsConnected)
            {
                deviceError = "Device disconnected.";
                this.DeviceDisconnected?.Invoke(this, null);
            }
            else if (response == null)
            {
                if (nullResponseCnt >= maxNullResponseCount)
                {
                    deviceError = $"Device is not responding.";
                    logger.LogError($"{deviceError}");
                    isAcceptorEnabled = false;
                    isAcceptorInitialized = false;
                    this.DeviceDisabled?.Invoke(this, null);
                    this.DeviceDisconnected?.Invoke(this, null);
                }
            }
        }
        else if (newStatus == AcceptorStatus.PowerUp)
        {
            if (isAcceptorInitialized == false)
            {
                if (this.IsConnected)
                {
                    await this.ResetAsync();
                    await this.EnableAsync();
                    isAcceptorInitialized = true;
                }
            }
        }
    }

    private async void PoolingDeviceForStatusResponses(CancellationTokenSource cancellationTokenSource)
    {
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            if (!this.IsConnected)
            {
                await Task.Delay(1000, cancellationTokenSource.Token).ContinueWith(async task =>
                {
                    try
                    {
                        var connectionOpened = await this.OpenConnectionAsync(this.portConfiguration);
                        if (connectionOpened == false)
                        {
                            logger.LogError("Reconnecting failed!");
                        }
                        else
                        {
                            logger.LogInformation("Device successfully reconnected!");
                        }
                    }
                    catch (Exception)
                    {
                        logger.LogError("Reconnecting failed!");
                    }
                });
                HandleAcceptorStatusChange(AcceptorStatus.Unknown);
                continue;
            }

            var statusResponse = await this.SendAndReceiveMessageAsync(new GetStatusCommand(), 200);

            nullResponseCnt = (statusResponse == null) ? ++nullResponseCnt : 0;

            if (statusResponse == null || statusResponse is not IAcceptorResponse)
            {
                await Task.Delay(200, cancellationTokenSource.Token).ContinueWith(task => { });

                if (nullResponseCnt > maxNullResponseCount)
                {
                    HandleAcceptorStatusChange(AcceptorStatus.Unknown);
                }
                continue;
            }
            else if (statusResponse is IdlingResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.ReadyForAccepting);
            }
            else if (statusResponse is RejectingResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.Rejecting, statusResponse as IAcceptorResponse);
            }
            else if (statusResponse is ReturningResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.Rejecting, statusResponse as IAcceptorResponse);
            }
            else if (statusResponse is AcceptingResponse || statusResponse is StackingResponse)
            {
                if (statusResponse is AcceptingResponse && this.BillAcceptingStarted != null)
                {
                    currentAcceptingMedia = AcceptingMedia.Bill;
                    this.BillAcceptingStarted?.Invoke(this, null);
                }

                HandleAcceptorStatusChange(AcceptorStatus.Accepting, statusResponse as IAcceptorResponse);
            }
            else if (statusResponse is EscrowResponse escrowResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.Accepting, statusResponse as IAcceptorResponse);
                currentAcceptingMedia = escrowResponse.IsCoupon ? AcceptingMedia.Ticket : AcceptingMedia.Bill;

                logger.LogInformation($"Accepting bill/ticket: {escrowResponse}");

                if (Mode == TerminalOperatingMode.Operator)
                {
                    logger.LogDebug("Terminal is in Operator mode.");
                    this.SendMessageAsync(new ReturnCommand());
                }
                else
                {
                    if (escrowResponse.IsCoupon)
                    {
                        currentAcceptingMedia = AcceptingMedia.Ticket;

                        if (this.TicketAccepted == null)
                        {
                            logger.LogError("TicketAccepted event is not subscribed.");
                            this.SendMessageAsync(new ReturnCommand());
                        }

                        if (billAcceptorConfiguration.AcceptTITOTickets == false)
                        {

                            logger.LogError("Ticket accepting is disabled.");
                            this.SendMessageAsync(new ReturnCommand());
                        }
                        else
                        {
                            var isTicketRedeemed = await this.RedeemTicketAsync(escrowResponse.Barcode);
                            if (isTicketRedeemed)
                            {
                                var receivingData = await this.SendAndReceiveMessageAsync(new Stack1Command(), 200);
                                if (receivingData is AcknowledgeResponse)
                                {
                                    this.TicketAccepted?.Invoke(this, escrowResponse.Barcode);
                                }
                            }
                            else
                            {
                                this.TicketRejected?.Invoke(this, escrowResponse.Barcode);
                                this.SendMessageAsync(new ReturnCommand());
                            }
                        }
                    }
                    else
                    {
                        currentAcceptingMedia = AcceptingMedia.Bill;

                        decimal escrowAmount = await this.AcceptBillAsync(escrowResponse.DenomKey);
                        if (escrowAmount > 0 || this.BillAccepted == null)
                        {
                            var receivingData = await this.SendAndReceiveMessageAsync(new Stack1Command(), 200);
                            if (receivingData is AcknowledgeResponse)
                            {
                                this.BillAccepted?.Invoke(this, escrowAmount);
                            }
                        }
                        else
                        {
                            if (this.BillAccepted == null)
                            {
                                logger.LogError("BillAccepted event is not subscribed.");
                            }

                            this.SendMessageAsync(new ReturnCommand());
                        }
                    }
                }
            }
            else if (statusResponse is VendValidResponse)
            {
                this.SendMessageAsync(new AcknowledgeCommand());
            }
            else if (statusResponse is StackedResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.Stacked);
            }
            else if (statusResponse is InhibitResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.Disabled);
            }
            else if (statusResponse is StackerOpenResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.StackerOpened);
            }
            else if (statusResponse is InitializeResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.Initializing);
            }
            else if (statusResponse is IPowerUpStatusResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.PowerUp, statusResponse as IPowerUpStatusResponse);
            }
            else if (statusResponse is IErrorStatusResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.Error, statusResponse as IErrorStatusResponse);
            }
            else if (statusResponse is AcknowledgeResponse)
            {
                // Do nothing
            }
            else if (statusResponse is DeviceDisabledResponse)
            {
                HandleAcceptorStatusChange(AcceptorStatus.Disabled);
            }
            else
            {
                HandleAcceptorStatusChange(AcceptorStatus.Unknown, (IAcceptorResponse)statusResponse);
            }
        }
    }

    public async Task<bool> InitializeAsync()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: LocalDevEnv - Bill Acceptor initialized and enabled. :::");
            isAcceptorInitialized = true;
            isAcceptorEnabled = true;
            currentStatus = AcceptorStatus.ReadyForAccepting;
            DeviceEnabled?.Invoke(this, null);
            await Task.Delay(500);
            return true;
        }

        if (this.portConfiguration == null)
        {
            throw new InvalidOperationException("Port configuration is not set.");
        }

        AcceptorResponseMessageFactory.Initialize();

        var connectionOpened = await this.OpenConnectionAsync(this.portConfiguration);
        if (connectionOpened == false)
        {
            return false;
        }
        await this.ResetAsync();
        await this.EnableAsync();
        if (this.billAcceptorStatusPoolingThread.ThreadState == ThreadState.Unstarted)
        {
            this.billAcceptorStatusPoolingThread.Start();
        }
        this.isAcceptorInitialized = true;

        return true;
    }

    protected override ISerialPortMessage ReadMessageInternal()
    {
        byte[] responseData = null;
        try
        {
            byte[] header = ReadSerialPortMessageBytes(2, 200);
            byte[] message = null;
            byte[] crc = null;

            if (header == null || header.Length != 2)
            {
                return null;
            }

            if (header[0] == this.syncByte)
            {
                int len = header[1];
                byte[] tmpResponse = null;
                message = ReadSerialPortMessageBytes(len - packageFramingSize, 500);
                crc = ReadSerialPortMessageBytes(2, 200);
                tmpResponse = ByteHelper.ConcatenateByteArrays(header, message);
                responseData = ByteHelper.ConcatenateByteArrays(tmpResponse, crc);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while reading data from Serial port!");
            return null;
        }

        if (responseData == null || responseData.Length == 0)
        {
            return null;
        }

        if (CheckChecksum(responseData) == false)
        {
            //throw new InvalidOperationException("CRC checksum is not valid.");
            return null;
        }

        int msgLength = responseData.Length - packageFramingSize;
        byte[] messageBytes = new Span<byte>(responseData).Slice(2, msgLength).ToArray();

        return AcceptorResponseMessageFactory.Current.TryCreateAcceptorResponse(messageBytes);
    }

    protected override bool SendMessageInternal(byte[] message)
    {
        using var memoryStream = new MemoryStream();
        memoryStream.WriteByte(this.syncByte);

        //Calculate PackageLength
        memoryStream.WriteByte((byte)(message.Length + this.packageFramingSize));

        memoryStream.Write(message);

        byte[] calculatedCrcBytes = GetChecksumBytes(memoryStream.ToArray());

        memoryStream.Write(calculatedCrcBytes);

        try
        {
            this.WriteMessageBytesToSerialPort(memoryStream.ToArray());
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Error while writing data to Serial port!");
            return false;
        }

        return true;
    }

    protected override void Dispose(bool disposing)
    {
        // Stop the bill acceptor thread
        cancellationTokenSource.Cancel();
        // Call the base Dispose method
        base.Dispose(disposing);
    }

    public async Task<string> GetFirmwareVersionAsync()
    {
        var receivedData = await this.SendAndReceiveMessageAsync(new GetVersionRequest());
        if (receivedData is VersionResponse)
        {
            return (receivedData as VersionResponse).VersionInfo;
        }
        else
        {
            throw new InvalidOperationException("Invalid response received.");
        }
    }

    public async Task<bool> EnableAsync()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: LocalDevEnv - Bill Acceptor enabled. :::");
            isAcceptorEnabled = true;
            DeviceEnabled?.Invoke(this, null);
            await Task.Delay(500);
            return true;
        }

        byte[] config = [0x00];
        var receivedData = await this.SendAndReceiveMessageAsync(new SetInhibitCommand(true), 1000);
        if (receivedData is InhibitResponse)
        {
            isAcceptorEnabled = true;
            return true;
        }
        else
        {
            // TODO: add logging
            return false;
        }
    }

    public async Task<bool> DisableAsync()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: LocalDevEnv - Bill Acceptor disabled. :::");
            isAcceptorEnabled = false;
            DeviceDisabled?.Invoke(this, null);
            return true;
        }

        var receivedData = await this.SendAndReceiveMessageAsync(new SetInhibitCommand(false), 1000);
        if (receivedData is InhibitResponse)
        {
            isAcceptorEnabled = false;
            return true;
        }
        else
        {
            // TODO: add logging
            return false;
        }
    }

    private byte[] GetChecksumBytes(Span<byte> data)
    {
        ushort crc = ByteHelper.CalculateCrc(data.ToArray(), 0x1021, 0x0000, 0x0000, true, true);
        return BitConverter.GetBytes(crc);

    }

    private bool CheckChecksum(Span<byte> message)
    {
        try
        {
            var messageLengthWithoutChecksum = message.Length - this.checksumLength;

            var dataWithoutChecksum = message.Slice(0, messageLengthWithoutChecksum);

            var crcBytes = GetChecksumBytes(dataWithoutChecksum);

            if (crcBytes.AsSpan().SequenceEqual(message.Slice(message.Length - this.checksumLength)) == false)
            {
                // TODO: add logging
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }

    private async Task<bool> RedeemTicketAsync(string barcode)
    {
        bool result = false;

        if (billAcceptorConfiguration == null || !billAcceptorConfiguration.AcceptTITOTickets)
        {
            return false;
        }

        if (this.serviceProvider != null)
        {
            try
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
                    result = await mediator.Send(new RedeemTicketCommand { Barcode = barcode, TicketType = TicketType.TITO });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while redeeming ticket with barcode {barcode}.");
            }
        }

        return result;
    }

    private async Task<decimal> AcceptBillAsync(byte amountByte)
    {
        decimal result = 0;

        if (this.serviceProvider != null)
        {
            try
            {
                if (billAcceptorConfiguration == null || (billAcceptorConfiguration.BillDenominationConfig == null))
                {
                    throw new InvalidOperationException("Bill acceptor configuration is not set.");
                }

                using (var scope = this.serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
                    result = await mediator.Send(new AcceptBillCommand(amountByte, billAcceptorConfiguration?.BillDenominationConfig));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while accepting bill with denom key 0x{amountByte.ToString("X2")}.");
            }
        }

        return result;
    }

    public async Task<bool> ResetAsync()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: LocalDevEnv - Bill Acceptor reset. :::");
            isAcceptorInitialized = true;
            isAcceptorEnabled = true;
            currentStatus = AcceptorStatus.ReadyForAccepting;
            DeviceEnabled?.Invoke(this, null);
            await Task.Delay(500);
            return true;
        }

        try
        {
            ISerialPortMessage response = null;

            response = await this.SendAndReceiveMessageAsync(new ResetCommand());
            if (response == null) return false;

            await Task.Delay(5000);

            var disabledConfig = new byte[] { 0x00, 0x00 };
            response = await this.SendAndReceiveMessageAsync(new EnableDisableDenominationCommand(disabledConfig));
            if (response == null) return false;

            await Task.Delay(500);

            var securityConfig = new byte[] { 0x00, 0x00 };
            response = await this.SendAndReceiveMessageAsync(new SetSecurityDenominationCommand(securityConfig));
            if (response == null) return false;

            await Task.Delay(500);

            response = await this.SendAndReceiveMessageAsync(new SetInhibitCommand(false));
            if (response == null) return false;

            await Task.Delay(500);

            var directionConfig = new byte[] { 0x00 };
            response = await this.SendAndReceiveMessageAsync(new SetDirectionCommand(directionConfig));
            if (response == null) return false;

            await Task.Delay(500);


            var functionConfig = new byte[] { 0x00, 0x00 };
            response = await this.SendAndReceiveMessageAsync(new SetOptionalFunctionCommand(functionConfig));
            if (response == null) return false;

            await Task.Delay(500);

            var modeConfig = new byte[] { 0x00 };
            response = await this.SendAndReceiveMessageAsync(new SetCommunicationModeCommand(modeConfig));
            if (response == null) return false;

            await Task.Delay(500);

            byte[] barcodeFunctionsConfig = [0x01, 0x12];
            response = await this.SendAndReceiveMessageAsync(new SetBarcodeFunctionsCommand(barcodeFunctionsConfig));
            if (response == null) return false;

            await Task.Delay(500);

            byte[] barcodeInhibitConfig = [0x00];
            response = await this.SendAndReceiveMessageAsync(new SetBarcodeInhibitCommand(barcodeInhibitConfig));
            if (response == null) return false;

            await Task.Delay(500);

            if (this.isAcceptorEnabled)
            {
                response = await this.SendAndReceiveMessageAsync(new SetInhibitCommand(true));
                if (response == null) return false;
                await Task.Delay(500);
            }
        }
        catch (Exception ex)
        {
            // TODO: add logging
            logger.LogError(ex, "Error while resetting the bill acceptor.");
            return false;
        }

        return true;
    }

    public Task<bool> ResetAsync(IBasicHardwareDeviceConfiguration configuration)
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: LocalDevEnv - Bill Acceptor reset. :::");
            isAcceptorInitialized = true;
            isAcceptorEnabled = true;
            currentStatus = AcceptorStatus.ReadyForAccepting;
            DeviceEnabled?.Invoke(this, null);
            Task.Delay(500);
            return Task.FromResult(true);
        }

        if (configuration is BillAcceptorID003Configuration billAcceptorConfiguration)
        {
            this.billAcceptorConfiguration = billAcceptorConfiguration;
            return this.ResetAsync();
        }

        throw new InvalidOperationException("Invalid configuration type.");
    }

    public string GetAdditionalDeviceInfo()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            return "::: LocalDevEnv - Bill Acceptor Device info... :::";
        }

        string fwVersion;

        try
        {
            fwVersion = this.GetFirmwareVersionAsync().Result;
        }
        catch (Exception)
        {
            fwVersion = "N/A";
        }

        string info = $"Name: {this.Name}  " + Environment.NewLine +
                      $"Port: {this.portConfiguration?.PortName}  " + Environment.NewLine +
                      $"Baud Rate: {this.portConfiguration?.BaudRate}  " + Environment.NewLine +
                      $"Firmware Version: {fwVersion} " + Environment.NewLine;

        return info;
    }

    public async Task<OperationResult> RunDiagnosticsCommand(DeviceDiagnosticsCommand command, params object[] args)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command), "Command cannot be null.");
        }

        if (SupportedDiagnosticCommands.Any(c => c.Code == command.Code) == false)
        {
            throw new NotSupportedException($"Command {command.Code} is not supported by the Bill Acceptor Driver.");
        }

        if (CommandInProgress)
        {
            return new OperationResult
            {
                IsSuccess = false,
                ErrorMessage = $"Command {command.Code} is already in progress."
            };
        }

        CommandInProgress = true;
        var result = new OperationResult();

        try
        {
            result.IsSuccess = command.Code switch
            {
                RESET_COMMAND => await this.ResetAsync(),
                ENABLE_COMMAND => await this.EnableAsync(),
                DISABLE_COMMAND => await this.DisableAsync(),
                _ => false
            };
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            CommandInProgress = false;
        }

        return result;
    }
}
