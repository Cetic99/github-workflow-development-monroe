using System.IO.Ports;
using System.Text;
using CashVault.CabinetControlUnitDriver.Config;
using CashVault.CabinetControlUnitDriver.Messages;
using CashVault.CabinetControlUnitDriver.Messages.RequestMessages;
using CashVault.CabinetControlUnitDriver.Messages.ResponseMessages;
using CashVault.DeviceDriver.Common;
using CashVault.DeviceDriver.Common.Helpers;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CashVault.CabinetControlUnitDriver
{
    public class CabinetControlUnitDriver : BaseSerialPortDriver, ICabinet
    {
        private readonly IServiceProvider serviceProvider;

        public TerminalOperatingMode Mode { get; private set; }

        public void SetOperatingMode(TerminalOperatingMode mode)
        {
            Mode = mode;
        }

        public string Name => "CashVault.CabinetControlUnit";
        private CabinetControlUnitConfiguration cabinetConfiguration;

        private bool isCabinetInitialized = false;
        private bool isCabinetConnected = false;
        private bool isCabinetEnabled = false;

        public event EventHandler<string> WarningRaised;
        public event EventHandler<string> ErrorOccured;
        public event EventHandler DeviceDisabled;
        public event EventHandler DeviceEnabled;
        public event EventHandler DeviceDisconnected;
        public event EventHandler<int> DoorOpened;
        public event EventHandler<int> DoorClosed;
        public event EventHandler VibrationDetected;
        private bool vibrationDetectedFlag = false;

        private SemaphoreSlim deviceAccessSemaphore = new SemaphoreSlim(1, 1); // Controls access to the device
        private SemaphoreSlim statusUpdateSemaphore = new SemaphoreSlim(1, 1);
        private Timer statusCheckTimer;
        private readonly TimeSpan _statusCheckInterval = TimeSpan.FromSeconds(1); // Set the check interval

        volatile string currentDeviceErrors = "";
        volatile string currentDeviceWarnings = "";
        volatile string currentDeviceStatus = "";
        volatile string firmwareVersion = "";
        private readonly int checksumLength = 2;

        private Dictionary<int, bool> tmpDoorSensorsStatus;

        const float INVALID_TEMPERATURE = -199.9f;
        public float Temperature { get; private set; } = INVALID_TEMPERATURE; // Default invalid temperature value

        // Initialize the timer to periodically check the device status
        private void InitializeStatusCheckTimer()
        {
            statusCheckTimer = new Timer(async _ => await PerformPeriodicDeviceCheckAsync(), null, Timeout.Infinite, Timeout.Infinite);
        }

        public CabinetControlUnitDriver(Port port, CabinetControlUnitConfiguration cabinetConfiguration, IServiceProvider serviceProvider, LocalDevEnvOptions? localDevEnvOptions = null)
           : base(serviceProvider.GetRequiredService<ILogger<CabinetControlUnitDriver>>(), localDevEnvOptions)
        {
            this.serviceProvider = serviceProvider;

            if (port.PortType != PortType.Serial)
            {
                throw new ArgumentException("Invalid port type for the bill dispenser driver.");
            }

            var availableBaudRates = new List<int> { 1200, 2400, 4800, 9600, 19200, 38400 };
            if (availableBaudRates.Contains(cabinetConfiguration.BaudRate) == false)
            {
                throw new ArgumentException("Invalid baud rate for the ticket printer driver.");
            }

            this.portConfiguration = new()
            {
                PortName = port.SystemPortName,
                BaudRate = cabinetConfiguration.BaudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadTimeout = 2000,
                WriteTimeout = 2000
            };

            this.cabinetConfiguration = cabinetConfiguration;
            InitializeStatusCheckTimer();
            var terminal = serviceProvider.GetRequiredService<ITerminal>();
            this.SetOperatingMode(terminal.OperatingMode);
            this.WarningRaised = OnWarningRaised;

            if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
            {
                isCabinetInitialized = true;
                isCabinetConnected = true;
                isCabinetEnabled = true;
            }
        }

        private void OnWarningRaised(object sender, string message)
        {
            logger.LogWarning($"Warning: {message}");
        }

        public bool IsInitialized => this.isCabinetInitialized;
        public bool IsConnected => this.isCabinetConnected;
        public bool IsEnabled => this.isCabinetEnabled;
        public bool IsActive => IsConnected && IsEnabled;

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                isCabinetEnabled = false;
                statusCheckTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task<bool> InitializeAsync()
        {
            if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
            {
                logger.LogInformation("::: LocalDevEnv - Cabinet Control Unit initialized and enabled. :::");
                isCabinetInitialized = true;
                isCabinetConnected = true;
                isCabinetEnabled = true;
                await Task.Delay(500);
                this.DeviceEnabled?.Invoke(this, null);
                return true;
            }

            if (this.portConfiguration == null)
            {
                throw new InvalidOperationException("Cabinet control unit port configuration is not set.");
            }

            if (isCabinetConnected == false)
            {
                var connectionOpened = await this.OpenConnectionAsync(this.portConfiguration);
                if (connectionOpened == false)
                {
                    logger.LogError($"Cabinet Control Unit connection failed.");
                    this.DeviceDisconnected?.Invoke(this, null);
                    return false;
                }
                else
                {
                    isCabinetEnabled = true;
                    this.DeviceEnabled?.Invoke(this, null);
                }
            }

            bool ret = false;
            while (isCabinetEnabled && !ret)
            {
                // Initialization loop
                await Task.Delay(100);
                ret = await InitialDoorSensorsCheck();
            }

            if (ret)
            {
                isCabinetInitialized = true;
                logger.LogInformation("Cabinet control unit device initialized/sensor status read.");

                var fwVersionCheck = await this.SendAndReceiveMessageAsync(new FirmwareVersionRequest());

                if (fwVersionCheck is FirmwareVersionResponse response)
                {
                    firmwareVersion = Encoding.UTF8.GetString(response.PAYLOAD);
                    logger.LogDebug($"Cabinet control unit firmware version: {firmwareVersion}\n");
                }
                else if (fwVersionCheck is InvalidCommandResponse invResponse)
                {
                    logger.LogInformation($"Invalid command response - command {invResponse.PAYLOAD[0]} failed!\n");
                    currentDeviceStatus = "Initialization failed!";
                    return false;
                }
                else
                {
                    logger.LogError("Error while reading firmware version.");
                    currentDeviceErrors = "Initialization failed!";
                    return false;
                }

                isCabinetConnected = true;
                StartDeviceStatusMonitoring();
                return true;
            }
            else
            {
                logger.LogInformation("Cabinet control unit device initialization failed.");
                isCabinetInitialized = false;
                currentDeviceErrors = "Initialization failed!";
                return false;
            }
        }

        public async Task<bool> EnableAsync()
        {
            if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
            {
                logger.LogInformation("::: LocalDevEnv - Cabinet Control Unit enabled. :::");
                isCabinetEnabled = true;
                await Task.Delay(500);
                this.DeviceEnabled?.Invoke(this, null);
                return true;
            }

            if (this.isCabinetEnabled)
            {
                logger.LogInformation("Already enabled.");
            }
            else
            {
                if (!isCabinetInitialized)
                {
                    this.isCabinetEnabled = await InitializeAsync();
                }
                else
                {
                    this.isCabinetEnabled = true;
                    this.DeviceEnabled?.Invoke(this, null);
                }
                logger.LogInformation("Enabled successfully.");
            }

            return this.isCabinetEnabled;
        }

        public async Task<bool> DisableAsync()
        {
            if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
            {
                logger.LogInformation("::: LocalDevEnv - Cabinet Control Unit disabled. :::");
                isCabinetEnabled = false;
                this.DeviceDisabled?.Invoke(this, null);
                return true;
            }

            //Simulate async method
            await Task.Delay(100);
            if (isCabinetEnabled == false)
            {
                logger.LogInformation("Already disabled.");
            }
            else
            {
                isCabinetEnabled = false;
                this.DeviceDisabled?.Invoke(this, null);
                logger.LogInformation("Cabinet control unit disabled successfully.");
            }

            return true;
        }

        public Task<bool> ResetAsync()
        {
            if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
            {
                logger.LogInformation("::: LocalDevEnv - Cabinet Control Unit reset. :::");
                isCabinetInitialized = true;
                isCabinetConnected = true;
                isCabinetEnabled = true;
                Task.Delay(500);
                return Task.FromResult(true);
            }

            bool ret = true;

            return Task.FromResult(ret);
        }

        public Task<bool> ResetAsync(IBasicHardwareDeviceConfiguration configuration)
        {
            bool ret = true;

            return Task.FromResult(ret);
        }

        public async Task<string> GetCurrentStatus()
        {
            //Simulate async method
            await Task.Delay(100);
            string message;

            if (IsEnabled)
            {
                message = currentDeviceStatus;
            }
            else
            {
                message = "Cabinet control unit not enabled!";
            }

            return message;
        }

        public string GetWarning()
        {
            return currentDeviceWarnings;
        }

        public string GetError()
        {
            return currentDeviceErrors;
        }

        public string GetAdditionalDeviceInfo()
        {
            if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
            {
                return "::: LocalDevEnv - Cabinet Control Unit Device info... :::";
            }

            string info = $"Name: {this.Name} " + Environment.NewLine +
                          $"Port: {this.portConfiguration?.PortName} " + Environment.NewLine +
                          $"BaudRate: {this.portConfiguration?.BaudRate} " + Environment.NewLine +
                          $"Firmware version: {this.firmwareVersion} " + Environment.NewLine;

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

        private async Task<bool> DeviceStatus()
        {
            if (IsEnabled)
            {
                try
                {
                    var CCUStatus = await this.SendAndReceiveMessageAsync(new CabinetControlUnitStatusRequest());

                    if (CCUStatus is CabinetControlUnitStatusResponse response)
                    {
                        // Errors
                        if (response.Errors != null && response.Errors.Count > 0)
                        {
                            currentDeviceErrors = "";
                            foreach (var item in response.Errors)
                            {
                                currentDeviceErrors += item.Description;
                            }
                        }

                        // Warnings
                        if (response.Warnings != null && response.Warnings.Count > 0)
                        {
                            currentDeviceWarnings = "";
                            foreach (var item in response.Warnings)
                            {
                                currentDeviceWarnings += item.Description;
                            }
                        }

                        // Handle vibration detection
                        if (response.Warnings.Any(w => w.Code == WarningCodes.VIBRATION_DETECTED))
                        {
                            if (vibrationDetectedFlag == false)
                            {
                                vibrationDetectedFlag = true;
                                VibrationDetected?.Invoke(this, null);
                                logger.LogWarning("Vibration detected!");
                            }
                        }
                        else
                        {
                            vibrationDetectedFlag = false;
                        }

                        if (response.PAYLOAD[0] == 0x00)
                        {
                            currentDeviceStatus = "Status OK";
                            currentDeviceErrors = "";  // Clear the errors
                            currentDeviceWarnings = "";  // Clear the warnings
                            logger.LogDebug($"Device status - OK\n");
                        }
                        else
                        {
                            currentDeviceStatus = "Status: err/wrn code detected!";
                        }

                        return true;
                    }
                    else if (CCUStatus is InvalidCommandResponse invResponse)
                    {
                        logger.LogInformation($"Invalid command response - command {invResponse.PAYLOAD[0]} failed!\n");
                        return false;
                    }
                    else
                    {
                        logger.LogDebug("No status response from CCU device!");
                        if (isCabinetConnected)
                        {
                            isCabinetConnected = false;
                        }
                        return false;
                    }

                }
                catch (InvalidOperationException ex)
                {
                    logger.LogInformation($"InvalidOperationException {ex.Message}");
                    return false;
                }
            }
            else
            {
                logger.LogError($"Cabinet control unit is disabled!");
                return false;
            }
        }

        // Perform the device check
        private async Task PerformPeriodicDeviceCheckAsync()
        {
            if (IsEnabled)
            {
                await deviceAccessSemaphore.WaitAsync(); // Wait for exclusive access to the device
                try
                {
                    logger.LogDebug("Starting periodic status check...");

                    bool deviceStatus = await DeviceStatus();

                    if (!deviceStatus)
                    {
                        logger.LogError("Device status check failed.");

                        if (isCabinetConnected == false)
                        {
                            // Try to reconnect the device
                            await DeviceReconnectLoop();
                        }
                    }
                    else
                    {
                        // logger.LogDebug("Device status check - successful.");
                    }

                    //Periodic door sensors check
                    var sensorsCheck = await InternalAllDoorSensorsStatusesCheck();

                    if (!sensorsCheck)
                    {
                        logger.LogError("Door sensors status check error!\n");
                    }

                    await TemperatureCheck();
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
            statusCheckTimer.Change(TimeSpan.Zero, _statusCheckInterval); // Restart the timer
            logger.LogInformation("Cabinet control unit status monitoring started.");
        }
        // Stop the status monitoring
        public void StopDeviceStatusMonitoring()
        {
            statusCheckTimer.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer
            logger.LogInformation("Cabinet control unit status monitoring stopped.");
        }

        public async Task<bool> CheckAllDoorSensorsStatuses()
        {
            if (IsActive)
            {
                await deviceAccessSemaphore.WaitAsync(); // Wait for exclusive access to the device
                try
                {
                    var allDoorSensorsStatus = await this.SendAndReceiveMessageAsync(new AllDoorSensorsStatusRequest());

                    if (allDoorSensorsStatus is AllDoorSensorsResponse response)
                    {
                        logger.LogDebug("Received response for all door sensors!");

                        foreach (var item in response.SensorStatus)
                        {
                            if (item.Opened)
                            {
                                this.DoorOpened?.Invoke(this, item.SensorId);
                                logger.LogWarning($"Door {item.SensorId} : Opened!\n");
                            }
                            else
                            {
                                this.DoorClosed?.Invoke(this, item.SensorId);
                                logger.LogWarning($"Door {item.SensorId} : Closed!\n");
                            }
                        }
                    }
                    else if (allDoorSensorsStatus is InvalidCommandResponse invResponse)
                    {
                        logger.LogDebug($"Invalid command response - command {invResponse.PAYLOAD[0]} failed!\n");
                        return false;
                    }
                    else
                    {
                        logger.LogDebug("No response from door sensors!");
                        return false;
                    }
                    return true;
                }
                finally
                {
                    deviceAccessSemaphore.Release(); // Release the semaphore after the status check
                    logger.LogDebug("Single door sensor status check completed.");
                }
            }
            else
            {
                logger.LogError("All door sensors status check failed - device not active!\n");
                return false;
            }
        }

        private async Task<bool> InternalAllDoorSensorsStatusesCheck()
        {
            //Periodic door sensors check
            var allDoorSensorsStatus = await this.SendAndReceiveMessageAsync(new AllDoorSensorsStatusRequest());

            if (allDoorSensorsStatus is AllDoorSensorsResponse response)
            {
                logger.LogDebug("Received response for all door sensors!");
                HandleAllDoorStatusChange(response);
            }
            else if (allDoorSensorsStatus is InvalidCommandResponse invResponse)
            {
                logger.LogDebug($"Invalid command response - command {invResponse.PAYLOAD[0]} failed!\n");
                return false;
            }
            else
            {
                logger.LogDebug("No response from door sensors!");
                return false;
            }
            return true;
        }

        public async Task<bool> CheckSpecificDoorSensorStatus(int sensorId)
        {
            if (IsActive)
            {
                await deviceAccessSemaphore.WaitAsync(); // Wait for exclusive access to the device
                try
                {
                    var specificDoorSensorStatus = await this.SendAndReceiveMessageAsync(new SpecificDoorSensorStatusRequest(sensorId));

                    if (specificDoorSensorStatus is SpecificDoorSensorResponse response)
                    {
                        logger.LogDebug("Received response for specific door sensor!");

                        if (response.doorStatus.Opened)
                        {
                            logger.LogInformation($"Door {response.doorStatus.SensorId} is opened!\n");
                        }
                        else
                        {
                            logger.LogInformation($"Door {response.doorStatus.SensorId} is closed!\n");
                        }
                    }
                    else if (specificDoorSensorStatus is InvalidCommandResponse invResponse)
                    {
                        logger.LogDebug($"Invalid command response - command {invResponse.PAYLOAD[0]} failed!\n");
                        return false;
                    }
                    else
                    {
                        logger.LogDebug("No response from door sensors!");
                        return false;
                    }
                    return true;
                }
                finally
                {
                    deviceAccessSemaphore.Release(); // Release the semaphore after the status check
                    logger.LogDebug("Single door sensor status check completed.");
                }
            }
            else
            {
                logger.LogError("Single door sensor status check failed - device not active!\n");
                return false;
            }
        }

        //Initialize door sensors status
        private async Task<bool> InitialDoorSensorsCheck()
        {
            var allDoorSensorsStatus = await this.SendAndReceiveMessageAsync(new AllDoorSensorsStatusRequest());

            if (allDoorSensorsStatus is AllDoorSensorsResponse response)
            {
                logger.LogDebug("Received response for all door sensors!");

                tmpDoorSensorsStatus = new Dictionary<int, bool>(response.MSG_LENGTH - 5);

                foreach (var item in response.SensorStatus)
                {
                    tmpDoorSensorsStatus[item.SensorId] = item.Opened;
                }
            }
            else if (allDoorSensorsStatus is InvalidCommandResponse invResponse)
            {
                // TODO: Check this case
                logger.LogDebug($"Invalid command response - command {invResponse.PAYLOAD[0]} failed!\n");
            }
            else
            {
                logger.LogError("Message for all door sensors status not found!");
                return false;
            }

            return true;
        }

        //Check door sensor status
        private void HandleAllDoorStatusChange(AllDoorSensorsResponse status)
        {
            foreach (var item in status.SensorStatus)
            {
                if (tmpDoorSensorsStatus[item.SensorId] != item.Opened)
                {
                    if (item.Opened)
                    {
                        this.DoorOpened?.Invoke(this, item.SensorId);
                        tmpDoorSensorsStatus[item.SensorId] = item.Opened;
                        logger.LogWarning($"Door {item.SensorId} : Opened!\n");
                    }
                    else
                    {
                        this.DoorClosed?.Invoke(this, item.SensorId);
                        tmpDoorSensorsStatus[item.SensorId] = item.Opened;
                        logger.LogWarning($"Door {item.SensorId} : Closed!\n");
                    }
                }
                else
                {
                    logger.LogDebug($"No door status change!\n");
                }
            }
        }

        protected override ISerialPortMessage ReadMessageInternal()
        {
            // maximum waiting time for response
            const int WAITING_TIMEOUT_MS = 3000;
            const byte responseHeader = 0x22;
            byte[] header = null;

            try
            {
                // Get first byte - header
                header = ReadSerialPortMessageBytes(1, WAITING_TIMEOUT_MS);
                Thread.Sleep(20);

                if (header[0] == responseHeader)
                {
                    // read next byte - msg length
                    byte[] msgLen = ReadSerialPortMessageBytes(1, WAITING_TIMEOUT_MS);

                    if (msgLen != null && msgLen[0] > 0)
                    {
                        byte[] msg = ReadSerialPortMessageBytes(msgLen[0] - 2, WAITING_TIMEOUT_MS);
                        if (msg != null && msg.Length == (msgLen[0] - 2))
                        {
                            byte[] response = ByteHelper.ConcatenateByteArrays(header, msgLen);

                            response = ByteHelper.ConcatenateByteArrays(response, msg);

                            //Check CRC
                            if (CheckChecksum(response))
                            {
                                var ret = CabinetControlUnitMessageFactory.Current.TryCreateCabinetControlUnitResponse(response);
                                return ret;
                            }
                            else
                            {
                                logger.LogError("Checksum error!\n");
                            }
                        }
                    }
                }
                //else { /* flush serial port ?? */ }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while reading Serial port from CabinetControlUnit driver. {ex.Message}");
                return null;
            }

            return null;
        }

        protected override bool SendMessageInternal(byte[] message)
        {
            try
            {
                if (message != null && message.Length > this.checksumLength)
                {
                    byte[] dataWithoutChecksum = message.Take(message.Length - this.checksumLength).ToArray();
                    byte[] crcBytes = GetChecksumBytes(dataWithoutChecksum);

                    message[message.Length - this.checksumLength] = crcBytes[0];
                    message[message.Length - this.checksumLength + 1] = crcBytes[1];
                }
                this.WriteMessageBytesToSerialPort(message);
            }
            catch (Exception ex)
            {
                Task.Delay(500);
                return false;
            }
            return true;
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

        private async Task<bool> ReopenSerialInternaly()
        {
            try
            {
                var connectionOpened = await this.OpenConnectionAsync(this.portConfiguration);
                if (connectionOpened == false)
                {
                    logger.LogDebug($"Serial Port initialization error.");
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
                logger.LogDebug($"Serial Port initialization error. {ex.Message}");
                return false;
            }
        }

        private async Task<bool> DeviceReconnectLoop()
        {
            isCabinetConnected = false;
            DeviceDisconnected?.Invoke(this, null);
            currentDeviceStatus = "Device disconnected!";
            currentDeviceWarnings = "Device disconnected, trying to reconnect!";

            logger.LogError("Device disconnected, trying to reconnect...");
            if (isCabinetEnabled) // maybe redundant...
            {
                while (isCabinetEnabled)
                {
                    if (IsSerialPortConnected == false)
                    {
                        if (await ReopenSerialInternaly() == false)
                        {
                            // serial port reopen failed, try again
                            continue;
                        }
                    }

                    try
                    {
                        // check comunication with CCU unit
                        var CCUStatus = await this.SendAndReceiveMessageAsync(new CabinetControlUnitStatusRequest());
                        if (CCUStatus is CabinetControlUnitStatusResponse response ||
                            CCUStatus is InvalidCommandResponse invResponse)
                        {
                            isCabinetConnected = true;
                            currentDeviceWarnings = ""; // Clear the warnings (currently only used for disconnected warning)
                            logger.LogInformation("CCU Reconnection successful!");
                            return true;
                        }

                    }
                    catch (ArgumentException ex)
                    {
                        // try again
                    }
                    await Task.Delay(500);
                }
                logger.LogError("Reconnection failed!");
            }
            else
            {
                logger.LogError("Device disabled. Can't reconnect!");
            }
            return false;
        }

        private async Task<float> TemperatureCheck()
        {
            var temperatureResponse = await this.SendAndReceiveMessageAsync(new TemperatureRequest());
            if (temperatureResponse is TemperatureResponse response)
            {
                Temperature = response.Temperature;
                return response.Temperature;
            }
            else if (temperatureResponse is InvalidCommandResponse invResponse)
            {
                logger.LogDebug($"Invalid command response - command {invResponse.PAYLOAD?[0]} failed!\n");
                Temperature = INVALID_TEMPERATURE;
                return INVALID_TEMPERATURE;
            }
            else
            {
                logger.LogDebug("Message for temperature status not found!");
                Temperature = INVALID_TEMPERATURE;
                return INVALID_TEMPERATURE;
            }
        }
    }
}
