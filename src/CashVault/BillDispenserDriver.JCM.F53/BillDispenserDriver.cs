using System.IO.Ports;
using CashVault.BillDispenserDriver.JCM.F53.Config;
using CashVault.BillDispenserDriver.JCM.F53.Messages;
using CashVault.BillDispenserDriver.JCM.F53.Messages.Common;
using CashVault.BillDispenserDriver.JCM.F53.Messages.RequestMessages;
using CashVault.BillDispenserDriver.JCM.F53.Messages.ResponseMessages;
using CashVault.DeviceDriver.Common;
using CashVault.DeviceDriver.Common.Helpers;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static CashVault.BillDispenserDriver.JCM.F53.Messages.Common.BillCountConfiguration;

namespace CashVault.BillDispenserDriver.JCM.F53;

public class BillDispenserDriver : BaseSerialPortDriver, IBillDispenser
{
    private static readonly byte[] ENQ = [0x10, 0x05];
    private static readonly byte[] STX = [0x10, 0x02];
    private static readonly byte[] ACK = [0x10, 0x06];
    private static readonly byte[] NAK = [0x10, 0x15];
    private static readonly byte[] ETX = [0x10, 0x03];

    private BillDispenserJcm53Configuration deviceConfiguration;
    public TerminalOperatingMode Mode { get; private set; }

    public void SetOperatingMode(TerminalOperatingMode mode)
    {
        Mode = mode;
    }

    //External Events
    public event EventHandler DeviceDisabled;
    public event EventHandler DeviceEnabled;
    public event EventHandler<string> ErrorOccured; //After this event is invoked device is not usable anymore - Critical error occurred
    public event EventHandler DeviceDisconnected;
    public event EventHandler<string> WarningRaised;

    //Internal Events
    public event EventHandler<string> DeviceMaintenanceNeeded;
    public event EventHandler<string> SensorMaintenanceNecessary;
    public event EventHandler<string> SensorReplacement;

    private bool isDispenserInitialized = false;
    private bool isDispenserConnected = false;
    private bool isDispenserEnabled = false;
    private bool isDispenserActive => isDispenserConnected && isDispenserEnabled;

    private bool reconnectionInProgress = false;

    private SemaphoreSlim deviceAccessSemaphore = new SemaphoreSlim(1, 1); // Controls access to the device
    private Timer statusCheckTimer;
    private readonly TimeSpan _statusCheckInterval = TimeSpan.FromSeconds(5); // Set the internal status check interval

    volatile string currentDispenserStatus;
    volatile string currentDeviceErrors = "";
    volatile string currentDeviceWarnings;
    volatile string previousDeviceWarnings = "";
    public string GetWarning()
    {
        if (currentDeviceErrors == "")
        {
            currentDeviceErrors = " ";
        }
        return currentDeviceWarnings;
    }

    public string GetError()
    {
        return currentDeviceErrors;
    }

    // Initialize the timer to periodically check the device status
    private void InitializeStatusCheckTimer()
    {
        statusCheckTimer = new Timer(async _ => await PerformPeriodicDeviceCheckAsync(), null, Timeout.Infinite, Timeout.Infinite);
    }


    public BillDispenserDriver(Port port, BillDispenserJcm53Configuration configuration, IServiceProvider serviceProvider, LocalDevEnvOptions? localDevEnvOptions = null)
        : base(serviceProvider.GetRequiredService<ILogger<BillDispenserDriver>>(), localDevEnvOptions)
    {
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
            ReadTimeout = 2000,
            WriteTimeout = 2000
        };

        this.deviceConfiguration = configuration;
        InitializeStatusCheckTimer();

        //Subscribe to event
        this.DeviceMaintenanceNeeded += OnDeviceMaintenanceNeeded;
        this.SensorMaintenanceNecessary += OnSensorMaintenanceNecessary;
        this.SensorReplacement += OnSensorReplacement;
        this.ErrorOccured += OnErrorOccured;

        var terminal = serviceProvider.GetRequiredService<ITerminal>();
        this.SetOperatingMode(terminal.OperatingMode);

        if (localDevEnvOptions.Enabled)
        {
            isDispenserConnected = true;
            isDispenserEnabled = true;
            isDispenserInitialized = true;
            currentDispenserStatus = "Device Status - OK! ::: LOCAL DEVELOPEMENT";
        }
    }


    //Local handlers
    private async void DeviceResetNeeded(string message)
    {
        logger.LogError($"Dispenser reset needed: {message}\n");

        try
        {
            // Perform necessary reset actions here
            var resetResult = await ResetAsync();

            if (resetResult)
            {
                logger.LogDebug("Dispenser reset completed successfully.");
            }
            else
            {
                logger.LogError("Dispenser reset failed.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Error occurred during device reset: {ex.Message}");
            this.ErrorOccured?.Invoke(this, "Failed to reset the device");
        }
    }
    private async void PowerLossRecovery(string message)
    {
        logger.LogError($" Power loss recovery: {message}\n");

        //Transaction used for recover from power loss state
        var transaction = new DispenserBillTransaction(1, 0, "Recovery transaction", "xx");

        transaction.AddItem(
            new DispenserBillTransactionItem(1, 100, 1)
        );

        //Transaction used to collect number of notes dispense during power loss
        var dispense = await this.DispenseCashRecovery(transaction);

    }
    private void OnDeviceMaintenanceNeeded(object sender, string message)
    {
        logger.LogWarning($"Dispenser maintenance needed: {message}");
    }

    private void OnSensorMaintenanceNecessary(object sender, string message)
    {
        logger.LogError($"Sensor maintenance necessary: {message}");
    }

    private void OnSensorReplacement(object sender, string message)
    {
        logger.LogCritical($"Sensor replacement needed: {message}");
    }

    private void OnErrorOccured(object sender, string message)
    {
        logger.LogCritical($"Critical error occured: {message}");
        StopDeviceStatusMonitoring();
        isDispenserEnabled = false;
    }

    public bool IsInitialized => this.isDispenserInitialized;
    public bool IsConnected => this.isDispenserConnected;
    public bool IsEnabled => this.isDispenserEnabled;
    public bool IsActive => this.isDispenserActive;
    public string Name => "CashVault.BillDispenserDriver.JCM.F53";

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

    protected override bool SendMessageInternal(byte[] message)
    {
        bool ret;
        try
        {
            ret = this.SendF53MessageWithCRC(message);

        }
        catch (Exception ex)
        {
            Task.Delay(1000);
            ret = false;
        }
        return ret;
    }

    protected override ISerialPortMessage ReadMessageInternal()
    {
        try
        {
            var responseBuffer = ReadSerialPortMessageBytes();

            if (responseBuffer == null || responseBuffer.Length == 0)
            {
                logger.LogError("There is no valid response.");
                throw new InvalidOperationException("There is no valid response.");
            }

            if (ByteHelper.Equals(ENQ, responseBuffer) == false)
            {
                logger.LogError("Device is not trying to send a valid message - ENQ.");
                throw new InvalidOperationException("Device is not trying to send a valid message.");
            }

            // Send ACK to the device
            WriteMessageBytesToSerialPort(ACK);

            responseBuffer = ReadSerialPortMessageBytes(4);

            if (ByteHelper.Equals(STX, responseBuffer.Take(2).ToArray()) == false)
            {
                logger.LogError("Device is not trying to send a valid message - STX.");
                throw new InvalidOperationException("Device is not trying to send a valid message.");
            }

            byte[] messageLengthBytes = responseBuffer.Skip(2).Take(2).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(messageLengthBytes);
            }
            ushort messageLength = BitConverter.ToUInt16(messageLengthBytes, 0);

            byte[] messageContent = ReadSerialPortMessageBytes(messageLength);

            responseBuffer = ReadSerialPortMessageBytes(4);

            if (ByteHelper.Equals(ETX, responseBuffer.Take(2).ToArray()) == false)
            {
                logger.LogError("Device is not trying to send a valid message - ETX.");
                throw new InvalidOperationException("Device is not trying to send a valid message.");
            }

            byte[] receivedCrcBytes = responseBuffer.Skip(2).Take(2).ToArray();

            byte[] inputForCrc = ByteHelper.ConcatenateByteArrays(messageContent, ETX);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(messageLengthBytes);
            }
            inputForCrc = ByteHelper.ConcatenateByteArrays(messageLengthBytes, inputForCrc);
            ushort calculatedCrc = ByteHelper.CalculateCrc(inputForCrc, 0x1021, 0x0000, 0x0000, true, true);
            byte[] calculatedCrcBytes = BitConverter.GetBytes(calculatedCrc);

            if (ByteHelper.Equals(calculatedCrcBytes, receivedCrcBytes) == false)
            {
                logger.LogError("CRC checksum is not valid.");
                throw new InvalidOperationException("CRC checksum is not valid.");
            }

            // Send ACK to the device
            WriteMessageBytesToSerialPort(ACK);

            logger.LogDebug($"BillDispenserMessageFactory - Trying to parse response message: {BitConverter.ToString(messageContent).Replace("-", "")}");
            return BillDispenserMessageFactory.Current.TryCreateDispenserResponse(messageContent);
        }
        catch (InvalidOperationException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError($"Read message internal failed - {ex.Message}");
            return null;
        }

    }

    //TODO: Check if this is needed we check device in device status
    public override void DeviceReadinessCheck(int maximumRetryCount = 5, long maximumWaitingTime = 10000)
    {
        // TODO: Implement check if device status is OK
        // TODO: Implement check if all cassettes are present and ready
        //throw new NotImplementedException();
    }

    private bool SendF53MessageWithCRC(byte[] messageBuffer, int timeoutMilliseconds = 5000)
    {

        if (IsSerialPortConnected == false)
        {
            logger.LogError("Serial port is not initialized.");
            throw new InvalidOperationException("Serial port is not initialized.");
        }

        if (messageBuffer == null)
        {
            throw new ArgumentNullException(nameof(messageBuffer));
        }

        WriteMessageBytesToSerialPort(ENQ);

        var responseBuffer = ReadSerialPortMessageBytes();

        if (responseBuffer == null || responseBuffer.Length == 0)
        {
            logger.LogError("There is no response.");
            throw new InvalidOperationException("There is no response.");
        }

        if (ByteHelper.Equals(NAK, responseBuffer) || ByteHelper.Equals(ACK, responseBuffer) == false)
        {
            logger.LogError("Device is disabled to receive messages.");
            throw new InvalidOperationException("Device is disabled to receive messages.");
        }

        byte[] length = BitConverter.GetBytes((ushort)messageBuffer.Length);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(length); // This will turn [0x08, 0x00] into [0x00, 0x08]
        }

        byte[] finalMessage = ByteHelper.ConcatenateByteArrays(length, messageBuffer);
        finalMessage = ByteHelper.ConcatenateByteArrays(finalMessage, ETX);

        // Calculate CRC-16 checksum. Required variant is CRC-16-KERMIT
        // Subject to calculation is: LEGTH (2 bytes) + MESSAGE + DLE (0x10) + ETX (0x03)
        ushort crc = ByteHelper.CalculateCrc(finalMessage, 0x1021, 0x0000, 0x0000, true, true);

        // Convert CRC-16 checksum to byte array
        byte[] crcBytes = BitConverter.GetBytes(crc);

        // Add CRC-16 checksum to final message
        finalMessage = ByteHelper.ConcatenateByteArrays(finalMessage, crcBytes);

        // Add start of text (STX) at the beginning of the message
        finalMessage = ByteHelper.ConcatenateByteArrays(STX, finalMessage);

        // Write the final message to the serial port
        WriteMessageBytesToSerialPort(finalMessage);

        // waiting for second ACK
        var finalResponse = ReadSerialPortMessageBytes();

        if (ByteHelper.Equals(ACK, finalResponse))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> ResetAsync()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: Local development environment detected. Simulate reset... :::");
            await Task.Delay(500); // Simulate some delay for reset
            return true;
        }

        var result = await InitializeAsync();

        if (result)
        {
            logger.LogDebug("Reset successful.");
        }
        else
        {
            logger.LogError("Reset failed.");
        }

        return result;
    }

    public async Task<bool> EnableAsync()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: Local development environment detected. Simulate enable... :::");
            isDispenserEnabled = true;
            isDispenserInitialized = true;
            isDispenserConnected = true;
            currentDispenserStatus = "Device Status - OK! ::: LOCAL DEVELOPEMENT";
            this.DeviceEnabled?.Invoke(this, null);
            return true;
        }

        bool enabled;

        if (!isDispenserInitialized)
        {
            enabled = await InitializeAsync();
        }
        else
        {
            isDispenserEnabled = true;
            this.DeviceEnabled?.Invoke(this, null);
            StartDeviceStatusMonitoring();
            enabled = true;
        }

        if (enabled)
        {
            logger.LogInformation("Enabled successful.");
        }
        else
        {
            logger.LogInformation("Enable failed.");
        }

        return enabled;
    }

    public async Task<bool> DisableAsync()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: Local development environment detected. Simulate disable... :::");
            isDispenserEnabled = false;
            currentDispenserStatus = "Device Status - Disabled! ::: LOCAL DEVELOPEMENT";
            this.DeviceDisabled?.Invoke(this, null);
            return true;
        }
        //Simulate async method
        await Task.Delay(100);
        if (isDispenserEnabled)
        {
            isDispenserEnabled = false;
            this.DeviceDisabled?.Invoke(this, null);
            logger.LogInformation("Disabled successful.");
        }
        else
        {
            logger.LogInformation("Already disabled.");
        }

        return true;
    }
    public async Task<string> GetCurrentStatus()
    {
        string message;

        if (isDispenserActive || (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled))
        {
            message = currentDispenserStatus;

        }
        else if (reconnectionInProgress == true)
        {
            message = "Device reconnection in progress!";
        }
        else
        {
            message = "Dispenser not connected/enabled!";
        }

        return message;
    }

    public async Task<bool> InitializeAsync()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: Local development environment detected. Simulate initialization... :::");
            isDispenserConnected = true;
            isDispenserEnabled = true;
            isDispenserInitialized = true;
            currentDispenserStatus = "Device Status - OK! ::: LOCAL DEVELOPEMENT";
            this.DeviceEnabled?.Invoke(this, null);
            return true;
        }
        isDispenserEnabled = true;

        if (this.portConfiguration == null)
        {
            throw new InvalidOperationException("Dispenser port configuration is not set.");
        }

        if (isDispenserConnected == false)
        {
            var connectionOpened = await this.OpenConnectionAsync(this.portConfiguration);

            if (connectionOpened == false)
            {
                logger.LogCritical($"Dispenser connection failed.");
                this.DeviceDisconnected?.Invoke(this, null);
                isDispenserEnabled = false;
                return false;
            }
            else
            {
                try
                {
                    var connectionCheck = await this.SendAndReceiveMessageAsync(new DeviceStatusInformationRequestMessage());

                    if (connectionCheck is DeviceStatusResponseMessage)
                    {
                        isDispenserConnected = true;
                        logger.LogInformation("Dispenser successfully connected.");
                    }
                    else
                    {
                        logger.LogInformation("Dispenser connection failed.");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    if (await DeviceReconnectLoop() == true)
                    {
                        logger.LogInformation($"Dispenser reconnected successfully!");
                    }
                }
            }
        }

        var initializationResponse = await this.SendAndReceiveMessageAsync(new BillDispenserDeviceInitializationRequestMessage(this.deviceConfiguration));
        logger.LogDebug($"Initialization response: {initializationResponse}");

        if (initializationResponse is DeviceInitializationResponseMessage response)
        {
            ResponseCommonPart commonPart = response.CommonPart;

            bool magnet = CheckMagnetDenomination(commonPart, this.deviceConfiguration);
            if (magnet)
            {
                return false;
            }

            if (response.IsPositive())
            {
                logger.LogInformation($"Dispenser initialized successfully.");

                isDispenserInitialized = true;
                isDispenserEnabled = true;
                this.DeviceEnabled?.Invoke(this, null);

                StartDeviceStatusMonitoring();

                foreach (var item in response.SensorLevel)
                {
                    if (item.SensorLevelValueNormal)
                    {
                        logger.LogDebug($"Sensor {item.SensorName} level normal!");
                    }
                    if (item.MaintenanceNecessary)
                    {
                        this.SensorMaintenanceNecessary?.Invoke(this, item.SensorName);
                    }
                    if (item.ReplacementNecessary)
                    {
                        this.SensorReplacement?.Invoke(this, item.SensorName);

                        return false;
                    }
                }

                return true;
            }
            else
            {
                HandleCommonPartErrors(commonPart);
                return false;
            }
        }
        else
        {
            logger.LogError($"DeviceInitializationResponseMessage not detected!");

            return false;
        }
    }

    public async Task<bool> DeviceStatus()
    {
        string errorStatus = "";
        string warningStatus = "";
        bool errors = false;

        if (isDispenserActive)
        {
            try
            {
                var statusResponse = await this.SendAndReceiveMessageAsync(new DeviceStatusInformationRequestMessage());
                logger.LogDebug($"Dispenser periodic status response: {statusResponse}");

                if (statusResponse is DeviceStatusResponseMessage response)
                {
                    ResponseCommonPart commonPart = response.CommonPart;

                    //Check for Errors in common part - Error Codes
                    if (commonPart.Errors.Any())
                    {
                        foreach (var item in commonPart.Errors)
                        {
                            string messageTemplate = $"Code: {item.Code}\n" +
                                                     $"Description: {item.Description}\n";
                            if (item.IsCritical || item.ResetNeeded)
                            {
                                errors = true;
                                errorStatus += messageTemplate;

                                // Log and invoke events based on error properties
                                logger.LogError(messageTemplate);
                            }
                            else if (item.MaintenanceNeeded)
                            {
                                warningStatus += messageTemplate;
                            }
                            else
                            {
                                logger.LogError($"Unclassified error occured: {messageTemplate}\n");
                            }
                        }
                    }
                    //Check for cassette errors
                    foreach (var configCassette in this.deviceConfiguration.BillCassettes)
                    {
                        var responseCassette = commonPart.BillCassetteStatuses
                            .FirstOrDefault(rc => rc.CassetteNumber == configCassette.CassetteNumber);

                        if (responseCassette.CassetteError.Any())
                        {

                            foreach (var cassetteError in responseCassette.CassetteError)
                            {
                                string cassetteTemplate = $"Code: {cassetteError.Code}\n" +
                                                          $"Description: {cassetteError.Description}\n";

                                if (cassetteError.IsCritical || cassetteError.ResetNeeded)
                                {
                                    errors = true;
                                    errorStatus += cassetteTemplate;

                                    // Log and invoke events based on error properties
                                    logger.LogError(cassetteTemplate);
                                }
                                else if (cassetteError.MaintenanceNeeded)
                                {
                                    warningStatus += cassetteTemplate;
                                }
                                else
                                {
                                    logger.LogError($"Unclassified error occured{cassetteTemplate}!\n");
                                }
                            }
                        }
                    }
                    //Check for classifiaction errors
                    foreach (var item in commonPart.ErrorClassification)
                    {
                        string errorClassification = $"Code: {item.Code}\n" +
                                                     $"Description: {item.Description}";

                        if (item.IsCritical)
                        {
                            errors = true;
                            errorStatus += errorClassification;
                        }
                        else if (item.MaintenanceNeeded)
                        {
                            warningStatus += errorClassification;
                        }
                        else
                        {
                            logger.LogError($"Unclassified error occured{errorClassification}!\n");
                        }
                    }

                    foreach (var item in commonPart.ErrorDetails)
                    {
                        string errorDetails = $"Code: {item.Code}\n" +
                                              $"Description: {item.Description}";

                        if (item.IsCritical)
                        {
                            errors = true;
                            errorStatus += errorDetails;
                        }
                        else if (item.MaintenanceNeeded)
                        {

                            warningStatus += item;
                        }
                        else
                        {
                            logger.LogError($"Unclassified error occured{errorDetails}!\n");
                        }
                    }

                    if (errors)
                    {
                        currentDispenserStatus = "Critical error occured!\n";
                    }
                    else
                    {
                        currentDispenserStatus = "Device Status - OK!\n";
                    }

                    //Global variable 
                    currentDeviceErrors = errorStatus;
                    currentDeviceWarnings = warningStatus;

                    //Internal handlers call
                    HandleCommonPartErrors(commonPart);
                    HandleCassetteStatus(commonPart);
                    HandleSensorStatus(commonPart);

                    bool magnet = CheckMagnetDenomination(commonPart, this.deviceConfiguration);
                    if (magnet) { return false; }

                }
                else
                {
                    logger.LogError($"DeviceStatusResponseMessage not detected!");
                    if (await DeviceReconnectLoop() == true)
                    {
                        logger.LogInformation($"Dispenser reconnected successfully!");
                    }

                    return true;
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.LogDebug($"InvalidOperationException {ex.Message}");

                if (await DeviceReconnectLoop() == true)
                {
                    logger.LogInformation($"Dispenser reconnected successfully!");
                }
            }
        }
        else
        {
            logger.LogError($"Dispenser is not active!");
            return false;
        }

        return true;
    }

    public async Task<OperationResult> DispenseCashAsync(DispenserBillTransaction transaction)
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: Local development environment detected. Simulate cash dispense... :::");
            await Task.Delay(500); // Simulate some delay for dispensing
            foreach (var item in transaction.Items)
            {
                item.SetOutcome(item.BillCountRequested, 0);
            }
            return new OperationResult(true);
        }

        await deviceAccessSemaphore.WaitAsync();
        var result = new OperationResult(true);
        string transactionErrors = "";

        try
        {
            if (Mode == TerminalOperatingMode.Operator)
            {
                logger.LogDebug("Terminal is in Operator mode.");
                result.IsSuccess = false;
                result.ErrorMessage = "Terminal is in Operator mode.";
                return result;
            }

            if (!isDispenserActive)
            {
                logger.LogError("Dispenser is not active!");
                result.IsSuccess = false;
                result.ErrorMessage = "\"Dispenser is not active!";
                return result;
            }

            Dictionary<int, (int countedBills, int rejectedBills)> outcomes = new();

            foreach (var item in transaction.Items)
            {
                outcomes[item.CassetteNumber] = (0, 0);
            }

            //Dispense bills one by one cassette
            foreach (var item in transaction.Items)
            {
                int totalRequested = item.BillCountRequested;

                while (totalRequested > 0)
                {
                    int countToSend = Math.Min(totalRequested, 20); // Max 20 per request

                    var billCountResponse = await SendAndReceiveMessageAsync(new BillCountRequestMessage(
                        new BillCountConfiguration
                        {
                            BillCount = new[]
                            {
                            new BillCountRequestMessageItem
                            {
                                CassetteId = item.CassetteNumber,
                                Count = countToSend,
                                MaxNumberOfCountReject = deviceConfiguration.MaxNumberOfCountReject,
                                PickRetriesOfCount = deviceConfiguration.PickRetriesOfCount
                            }
                            }
                        }));

                    if (!ProcessResponse(billCountResponse, outcomes))
                    {
                        if (billCountResponse is BillCountFrontResponseMessage response)
                        {
                            ResponseCommonPart commonPart = response.CommonPart;

                            foreach (var err in commonPart.Errors)
                            {
                                transactionErrors = string.Join(Environment.NewLine, commonPart.Errors.Select(e => e.Description));
                            }
                        }
                    }

                    totalRequested -= countToSend;
                }
            }

            //Update final outcomes in transaction items
            foreach (var (cassette, (dispensed, rejected)) in outcomes)
            {
                var transactionItem = transaction.Items.FirstOrDefault(i => i.CassetteNumber == cassette);
                if (transactionItem != null)
                {
                    transactionItem.SetOutcome(dispensed, rejected);
                    logger.LogDebug($" Final outcome: Cassette {cassette} -> Dispensed: {dispensed}, Rejected: {rejected}");
                }
            }

            // TODO: This should be refactored to use a more structured error handling approach
            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                result.IsSuccess = false;
                result.ErrorMessage = transactionErrors;
                return result;
            }
            else
            {
                result.IsSuccess = true;
                result.ErrorMessage = transactionErrors;
                return result;
            }
        }
        finally
        {
            deviceAccessSemaphore.Release();
            logger.LogDebug("Dispensing operation finished.");
        }
    }

    private bool ProcessResponse(object billCountResponse, Dictionary<int, (int countedBills, int billRejections)> outcomes)
    {
        if (billCountResponse is BillCountFrontResponseMessage response)
        {
            ResponseCommonPart commonPart = response.CommonPart;

            foreach (var item in response.BillCountStatuses)
            {
                if (outcomes.ContainsKey(item.CassetteNumber))
                {
                    outcomes[item.CassetteNumber] = (
                        outcomes[item.CassetteNumber].countedBills + item.NumOfCountedBills,
                        outcomes[item.CassetteNumber].billRejections + item.NumOfBillRejections
                    );
                }
                else
                {
                    outcomes[item.CassetteNumber] = (item.NumOfCountedBills, item.NumOfBillRejections);
                }
            }

            if (response.IsPositive())
            {
                logger.LogDebug($"Cash dispensed successfully.");
                return true;
            }
            else
            {
                logger.LogError($"Dispenser bill count command failed!\n");
                HandleCommonPartErrors(commonPart);
                return false;
            }
        }
        else
        {
            logger.LogError($"BillCountFrontResponseMessage not detected!");
            return false;
        }
    }


    public async Task<bool> RejectCash(DispenserBillTransaction transaction)
    {
        await deviceAccessSemaphore.WaitAsync(); // Wait for any ongoing status check to complete

        try
        {
            if (Mode == TerminalOperatingMode.Operator)
            {
                if (isDispenserActive)
                {
                    Dictionary<int, (int countedBills, int rejectedBills)> outcomes = new();

                    foreach (var item in transaction.Items)
                    {
                        outcomes[item.CassetteNumber] = (0, 0); // Initialize cassette outcomes
                    }

                    // Process each cassette separately
                    foreach (var item in transaction.Items)
                    {
                        int totalRequested = item.BillCountRequested;

                        while (totalRequested > 0)
                        {
                            int countToSend = Math.Min(totalRequested, 20); // Max 20 per request

                            var rejectCashResponse = await SendAndReceiveMessageAsync(new AutomaticallyRejectBillCountRequestMessage(
                                new BillCountConfiguration
                                {
                                    BillCount = new[]
                                    {
                                    new BillCountRequestMessageItem
                                    {
                                        CassetteId = item.CassetteNumber,
                                        Count = countToSend,
                                        MaxNumberOfCountReject = deviceConfiguration.MaxNumberOfCountReject,
                                        PickRetriesOfCount = deviceConfiguration.PickRetriesOfCount
                                    }
                                    }
                                }));

                            ProcessResponse(rejectCashResponse, outcomes);

                            totalRequested -= countToSend;
                        }
                    }

                    // Update transaction items with final outcomes
                    foreach (var (cassette, (counted, rejected)) in outcomes)
                    {
                        var transactionItem = transaction.Items.FirstOrDefault(i => i.CassetteNumber == cassette);
                        if (transactionItem != null)
                        {
                            transactionItem.SetOutcome(counted, rejected);
                            logger.LogDebug($" Final rejection outcome: Cassette {cassette} -> Rejected: {rejected}, Dispensed: {counted}");
                        }
                    }
                    return true;
                }
                else
                {
                    logger.LogError($"Dispenser is not active!");
                    return false;
                }
            }
            else
            {
                logger.LogDebug("Terminal is not in Operator mode.");
                return false;
            }
        }
        finally
        {
            deviceAccessSemaphore.Release();
            logger.LogDebug("Reject cash operation finished.");
        }
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

    private void HandleCassetteStatus(ResponseCommonPart commonPart)
    {
        foreach (var configCassette in this.deviceConfiguration.BillCassettes)
        {
            var responseCassette = commonPart.BillCassetteStatuses
                .FirstOrDefault(rc => rc.CassetteNumber == configCassette.CassetteNumber);

            string cassetteStatus = $"\nCassette status: \n" +
                                    $"Cassette number: {responseCassette.CassetteNumber}\n" +
                                    $"Cassette denomination: {responseCassette.CassetteDenomination}\n" +
                                    $"Number of status changes : {responseCassette.NumberOfStatusChanges}\n";

            logger.LogDebug(cassetteStatus);

            foreach (var error in responseCassette.CassetteError)
            {
                string errorMessage = $"\nCassette error code: {error.Code}\n" +
                                      $"Description : {error.Description}\n";

                if (error.IsCritical)
                {
                    logger.LogError(errorMessage);
                    this.ErrorOccured?.Invoke(this, error.Description);
                }
                else if (error.MaintenanceNeeded)
                {
                    logger.LogWarning(errorMessage);

                    if ((currentDeviceWarnings != previousDeviceWarnings) && this.deviceConfiguration.SendLowLevelWarning == true)
                    {
                        this.WarningRaised?.Invoke(this, currentDeviceWarnings);
                        previousDeviceWarnings = currentDeviceWarnings;
                    }
                }
                else
                {
                    logger.LogError($"Unclassified error occured: {error.Description}\n");
                }
            }
        }
    }

    private void HandleCommonPartErrors(ResponseCommonPart commonPart)
    {
        // Iterate through errors in CommonPart and log them
        foreach (var item in commonPart.Errors)
        {
            string messageTemplate = $"\nError occurred:\n" +
                                     $"Code: {item.Code}\n" +
                                     $"Description: {item.Description}\n";


            //Event is raised based on error severity
            if (item.IsCritical)
            {
                // Log and invoke events based on error properties
                logger.LogDebug(messageTemplate);

                //Added to handle some specific critical errors inside driver
                switch (item.Code)
                {
                    case "FC":
                        //Number of notes dispensed not available
                        PowerLossRecovery(item.Description);
                        break;
                    case "FD":
                        //Power loss during dispense
                        DeviceResetNeeded(item.Description);
                        break;
                    default:
                        this.ErrorOccured?.Invoke(this, item.Description);
                        break;
                }
            }
            else if (item.ResetNeeded)
            {
                DeviceResetNeeded(item.Description);
            }
            else if (item.MaintenanceNeeded)
            {
                logger.LogWarning(messageTemplate);
                if ((currentDeviceWarnings != previousDeviceWarnings) && this.deviceConfiguration.SendLowLevelWarning == true)
                {
                    this.WarningRaised?.Invoke(this, currentDeviceWarnings);
                    previousDeviceWarnings = currentDeviceWarnings;
                }
            }
            else
            {
                logger.LogError($"Unclassified error occured: {messageTemplate}\n");
            }
        }

        // Iterate through classification register in CommonPart and log them (detailed information about errors)
        foreach (var item in commonPart.ErrorClassification)
        {
            string errorClassification = $"\nError Classification:\n" +
                                         $"Code: {item.Code}\n" +
                                         $"Description: {item.Description}";

            logger.LogDebug(errorClassification);

            if (item.IsCritical)
            {
                this.ErrorOccured?.Invoke(this, item.Description);
            }
            else if (item.MaintenanceNeeded)
            {
                logger.LogWarning(errorClassification);
                if ((currentDeviceWarnings != previousDeviceWarnings) && this.deviceConfiguration.SendLowLevelWarning == true)
                {
                    this.WarningRaised?.Invoke(this, currentDeviceWarnings);
                    previousDeviceWarnings = currentDeviceWarnings;
                }
            }
            else
            {
                logger.LogError($"Unclassified error occured: {errorClassification}\n");
            }
        }

        // Iterate through error details in CommonPart and log them (detailed information about errors)
        foreach (var item in commonPart.ErrorDetails)
        {
            string errorDetails = $"\nError details:\n" +
                                  $"Code: {item.Code}\n" +
                                  $"Description: {item.Description}";

            logger.LogDebug(errorDetails);

            if (item.IsCritical)
            {
                this.ErrorOccured?.Invoke(this, item.Description);
            }
            else if (item.MaintenanceNeeded)
            {
                logger.LogWarning(errorDetails);
                if ((currentDeviceWarnings != previousDeviceWarnings) && this.deviceConfiguration.SendLowLevelWarning == true)
                {
                    this.WarningRaised?.Invoke(this, currentDeviceWarnings);
                    previousDeviceWarnings = currentDeviceWarnings;
                }
            }
            else
            {
                logger.LogError($"Unclassified error occured: {errorDetails}\n");
            }
        }
    }

    private void HandleSensorStatus(ResponseCommonPart commonPart)
    {
        foreach (var sensor in commonPart.SensorStatuses)
        {
            string senorStatus = $"\nSensor status:\n" +
                                 $"Code: {sensor.Code}\n" +
                                 $"Description: {sensor.Description}\n";

            logger.LogDebug(senorStatus);
        }
    }

    private async Task<bool> DispenseCashRecovery(DispenserBillTransaction transaction)
    {
        if (isDispenserActive)
        {
            BillCountConfiguration billCountConfiguration = new();

            billCountConfiguration.BillCount = transaction.Items.Select(item => new BillCountRequestMessageItem
            {
                CassetteId = item.CassetteNumber,
                Count = item.BillCountRequested,
                MaxNumberOfCountReject = deviceConfiguration.MaxNumberOfCountReject,
                PickRetriesOfCount = deviceConfiguration.PickRetriesOfCount
            }).ToArray();

            var billCountResponse = await this.SendAndReceiveMessageAsync(new BillCountRequestMessage(billCountConfiguration));
            logger.LogDebug($"Dispense cash response: {billCountResponse}");

            if (billCountResponse is BillCountFrontResponseMessage response)
            {
                ResponseCommonPart commonPart = response.CommonPart;

                foreach (var res in response.BillCountStatuses)
                {
                    logger.LogInformation($"Cassette ID {res.CassetteNumber} -> Dispensed: {res.NumOfCountedBills} ; Rejected:{res.NumOfBillRejections}");

                    var transactionItem = transaction.Items.FirstOrDefault(i => i.CassetteNumber == res.CassetteNumber);
                    if (transactionItem != null)
                    {
                        transactionItem.SetOutcome(res.NumOfCountedBills, res.NumOfBillRejections);
                    }
                }

                HandleCassetteStatus(commonPart);
                HandleCommonPartErrors(commonPart);

                return true;
            }
            else
            {
                logger.LogError($"BillCountFrontResponseMessage not detected!");

                return false;
            }
        }
        else
        {
            logger.LogError($"Dispenser is not active!");
            return false;
        }
    }

    // Perform the device status check
    private async Task PerformPeriodicDeviceCheckAsync()
    {
        if (isDispenserActive && !reconnectionInProgress)
        {
            await deviceAccessSemaphore.WaitAsync(); // Wait for exclusive access to the device
            try
            {
                logger.LogDebug("Starting periodic status check...");

                // Simulate the status check
                bool deviceStatus = await DeviceStatus();

                if (!deviceStatus)
                {
                    logger.LogError("Device status check failed.");
                }
                else
                {
                    logger.LogDebug("Device status check - successful.");
                }
            }
            finally
            {
                deviceAccessSemaphore.Release(); // Release the semaphore after the status check
                logger.LogDebug("Periodic status check completed.");
            }
        }
    }

    // Start the status monitoring
    public void StartDeviceStatusMonitoring()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: Local development environment detected. Simulate status monitoring... :::");
            statusCheckTimer.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer
        }
        statusCheckTimer.Change(TimeSpan.Zero, _statusCheckInterval); // Restart the timer
        logger.LogInformation("Dispenser status monitoring started.");
    }

    // Stop the status monitoring
    public void StopDeviceStatusMonitoring()
    {
        statusCheckTimer.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer
        logger.LogInformation("Dispenser status monitoring stopped.");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            statusCheckTimer.Dispose();
            //TODO: Check this one more time
            //deviceAccessSemaphore.Dispose();
        }

        base.Dispose(disposing);
    }

    private async Task<bool> DeviceReconnectLoop()
    {
        DeviceDisconnected?.Invoke(this, null);
        isDispenserConnected = false;
        reconnectionInProgress = true;

        if (isDispenserEnabled)
        {
            logger.LogError("Device disconnected, trying to reconnect...");

            //Check communication with dispenser
            while (isDispenserEnabled)
            {
                if (IsSerialPortConnected == false)
                {
                    if (await reopenSerialInternaly() == false)
                    {
                        continue;
                    }
                }

                try
                {
                    if (await this.SendAndReceiveMessageAsync(new CancelRequestMessage()) is CancelResponseMessage)
                    {
                        logger.LogInformation("Terminated previous message!");
                    }
                    else
                    {
                        logger.LogInformation("Termination failed!");
                    }
                    await Task.Delay(500);
                    if (await this.SendAndReceiveMessageAsync(new DeviceStatusInformationRequestMessage()) is DeviceStatusResponseMessage)
                    {
                        break;
                    }

                }
                catch (Exception ex)
                {
                    await Task.Delay(500);
                }

                logger.LogError("Reconnection failed!");
                await Task.Delay(500);

                if (isDispenserEnabled == false)
                {
                    reconnectionInProgress = false;
                    return false;
                }
            }

            isDispenserConnected = true;
            reconnectionInProgress = false;
            return true;
        }
        else
        {
            logger.LogError("Device disabled. Can't reconnect!");
            reconnectionInProgress = false;
            return false;
        }
    }
    private async Task<bool> reopenSerialInternaly()
    {
        try
        {
            var connectionOpened = await this.OpenConnectionAsync(this.portConfiguration);
            if (connectionOpened == false)
            {
                logger.LogError($"Serial Port initialization error.");
                await Task.Delay(500);
                return false;
            }
            else
            {
                logger.LogInformation("Serial Port successfully reopened.");
                await Task.Delay(500);
                return true;
            }
        }
        catch (ArgumentException ex)
        {
            await Task.Delay(500);
            return false;
        }
    }

    bool CheckMagnetDenomination(ResponseCommonPart response, BillDispenserJcm53Configuration configuration)
    {
        bool error = false;
        string magnetError = "";
        //currentDeviceErrors = "";
        if (configuration?.BillCassettes == null)
        {
            logger.LogWarning("BillCassettes in configuration is null or empty.");
            return false;
        }

        if (response?.BillCassetteStatuses == null)
        {
            logger.LogWarning("BillCassetteStatuses in response is null or empty.");
            return false;
        }

        foreach (var configCassette in configuration.BillCassettes)
        {
            var responseCassette = response.BillCassetteStatuses
                .FirstOrDefault(rc => rc.CassetteNumber == configCassette.CassetteNumber);

            if (responseCassette != null)
            {
                logger.LogDebug($"Cassette Number: {configCassette.CassetteNumber}");

                logger.LogDebug(
                    $"Config Magnets - A: {configCassette.DenominationMagnetStatus?.MagnetA}, " +
                    $"B: {configCassette.DenominationMagnetStatus?.MagnetB}, " +
                    $"C: {configCassette.DenominationMagnetStatus?.MagnetC}, " +
                    $"D: {configCassette.DenominationMagnetStatus?.MagnetD}");

                logger.LogDebug(
                    $"Response Magnets - A: {responseCassette.CassetteDenomination?.MagnetA}, " +
                    $"B: {responseCassette.CassetteDenomination?.MagnetB}, " +
                    $"C: {responseCassette.CassetteDenomination?.MagnetC}, " +
                    $"D: {responseCassette.CassetteDenomination?.MagnetD}");

                if (configCassette.DenominationMagnetStatus?.MagnetA != responseCassette.CassetteDenomination?.MagnetA ||
                    configCassette.DenominationMagnetStatus?.MagnetB != responseCassette.CassetteDenomination?.MagnetB ||
                    configCassette.DenominationMagnetStatus?.MagnetC != responseCassette.CassetteDenomination?.MagnetC ||
                    configCassette.DenominationMagnetStatus?.MagnetD != responseCassette.CassetteDenomination?.MagnetD)
                {
                    error = true;
                    magnetError = $"Mismatch detected for Cassette Number: {configCassette.CassetteNumber}\n";
                    logger.LogError(magnetError);
                    currentDeviceErrors += magnetError;
                }

            }
            else
            {
                logger.LogWarning($"No matching response cassette found for Cassette Number: {configCassette.CassetteNumber}");
                return false;
            }
        }

        if (error)
        {
            this.ErrorOccured?.Invoke(this, $"{currentDeviceErrors}");
            return true;

        }
        return false;
    }

    public Task<bool> ResetAsync(IBasicHardwareDeviceConfiguration configuration)
    {
        if (configuration is BillDispenserJcm53Configuration billDispenserConfiguration)
        {
            this.deviceConfiguration = billDispenserConfiguration;
            return this.ResetAsync();
        }

        throw new InvalidOperationException("Invalid configuration type.");
    }

    public string GetAdditionalDeviceInfo()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            return "::: Local development environment detected. Bill Dispenser Device info... :::";
        }

        //TODO: Implement firmware version
        string fwVersion = "N/A";

        string info = $"Name: {this.Name} " + Environment.NewLine +
                      $"Port: {this.portConfiguration?.PortName} " + Environment.NewLine +
                      $"BaudRate: {this.portConfiguration?.BaudRate} " + Environment.NewLine +
                      $"Firmware version: {fwVersion} " + Environment.NewLine;

        return info;
    }
}
