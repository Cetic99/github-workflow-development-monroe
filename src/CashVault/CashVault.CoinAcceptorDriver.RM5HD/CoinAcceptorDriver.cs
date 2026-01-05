using System.IO.Ports;
using CashVault.ccTalk.ccTalkBase.Devices;
using CashVault.ccTalk.CoinAcceptorBase;
using CashVault.ccTalk.Common.Exceptions;
using CashVault.CoinAcceptorDriver.RM5HD.Config;
using CashVault.DeviceDriver.Common;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CashVault.CoinAcceptorDriver.RM5HD;

public partial class CoinAcceptorDriver : CcTalkCoinAcceptorBase, ICoinAcceptor
{
    private readonly IServiceProvider serviceProvider;
    private ILogger logger;
    private CoinAcceptorRM5HDConfiguration configuration;
    private CoinIndex inhibitMask = CoinIndex.All;
    private bool isInitialized = false;
    private bool isConnected = false;
    private bool isEnabled = false;
    private bool _isDisposed = false;

    private volatile string deviceWarning = "";
    private volatile string deviceError = "";
    private volatile string deviceStatus = "Not initialized";
    private CctalkDeviceStatus currentStatus = CctalkDeviceStatus.OtherError;

    private readonly TimeSpan STATUS_CHECK_INTERVAL = TimeSpan.FromSeconds(1);
    private Timer statusCheckTimer;
    private bool reconnectionInProgress = false;
    private bool reactivatePollingAfterReconnect = false;
    private readonly SemaphoreSlim deviceAccessSemaphore = new SemaphoreSlim(1, 1);
    private CctalkDeviceStatus? latestStatus = null;
    private readonly LocalDevEnvOptions? localDevEnvOptions;
    private int consecutiveErrorCount = 0;
    private const int MAX_CONSECUTIVE_ERRORS_FOR_RECONECT = 3; // Number of consecutive otherError (no response) before attempting reconnection
    private bool _eventsRegistered = false; // Track if base class events are registered

    private const string RESET_COMMAND = "Reset";
    private const string ENABLE_COMMAND = "Enable";
    private const string DISABLE_COMMAND = "Disable";
    public bool CommandInProgress { get; private set; } = false;
    public IEnumerable<DeviceDiagnosticsCommand> SupportedDiagnosticCommands =>
        [
                new DeviceDiagnosticsCommand(RESET_COMMAND, "Reset"),
                new DeviceDiagnosticsCommand(ENABLE_COMMAND, "Enable"),
                new DeviceDiagnosticsCommand(DISABLE_COMMAND, "Disable"),
        ];

    private void InitializeStatusCheckTimer()
    {
        statusCheckTimer = new Timer(async _ => await PerformPeriodicDeviceCheckAsync(), null, Timeout.Infinite, Timeout.Infinite);
    }

    public CoinAcceptorDriver(Port port, CoinAcceptorRM5HDConfiguration coinAcceptorConfiguration, IServiceProvider serviceProvider, LocalDevEnvOptions? localDevEnvOptions = null)
        : base(port, serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        this.configuration = coinAcceptorConfiguration;
        logger = serviceProvider.GetRequiredService<ILogger<CoinAcceptorDriver>>();
        this.localDevEnvOptions = localDevEnvOptions;

        if (port.PortType != PortType.Serial)
        {
            throw new ArgumentException("CoinAcceptorDriver can only be used with Serial ports.");
        }

        this.portConfiguration = new()
        {
            PortName = port.SystemPortName,
            BaudRate = 9600,
            DataBits = 8,
            Parity = Parity.None,
            StopBits = StopBits.One,
            WriteTimeout = 2000,
            ReadTimeout = 2000,
        };

        base.RemoveEcho = true;
        var terminal = serviceProvider.GetRequiredService<ITerminal>();
        Mode = terminal.OperatingMode;

        InitializeStatusCheckTimer();
    }

    public string Name => "CashVault.ccTalkCoinAcceptor.RM5HD";

    public bool IsInitialized => this.isInitialized;

    public bool IsConnected => this.isConnected;

    public bool IsEnabled => this.isEnabled;

    public bool IsActive => this.isConnected && this.isEnabled;

    public event EventHandler<string>? CoinRejected;
    public event EventHandler<decimal>? CoinAccepted;
    public event EventHandler? DeviceDisabled;
    public event EventHandler? DeviceEnabled;
    public event EventHandler<string>? ErrorOccured;
    public event EventHandler<string>? WarningRaised;
    public event EventHandler? DeviceDisconnected;

    private void OnCoinAccepted(object? sender, CoinAcceptorCoinEventArgs e)
    {
        logger.LogInformation($"COIN accepted: {e.CoinValue} BAM | Coin Name: {e.CoinName} | Coin Code: {e.CoinCode} ");
        CoinAccepted?.Invoke(this, e.CoinValue);
    }

    private void OnErrMsgAccepted(object? sender, CoinAcceptorErrorEventArgs e)
    {
        // TODO: Clasify and handle different error types (e.g. temporary vs critical)
        // TODO: Handle coin reject (e.g. CoinRejected?.Invoke(this, reason) )
        // Coin reject is shown as error msg from device
        // Handle warnings (not all error msg are actually errors)
        this.deviceError = e.ErrorMessage;
        logger.LogError($"ERROR detected: ErrCode: {e.Error}, Msg: {e.ErrorMessage}");
        ErrorOccured?.Invoke(this, e.ErrorMessage);
    }

    public async Task<bool> DisableAsync()
    {
        if (localDevEnvOptions?.Enabled == true)
        {
            logger.LogInformation("::: Local development environment detected. Disabling device in Dev mode :::");
            isInitialized = true;
            isEnabled = false;
            isConnected = true;
            currentStatus = CctalkDeviceStatus.Ok;
            await Task.Delay(500); // Simulate some delay
            return true;
        }

        try
        {
            IsInhibiting = true;
            logger.LogInformation("Inhibiting is set to true. Device will be disabled.");
            EndPoll();

            isEnabled = false;
            DeviceDisabled?.Invoke(this, null);

            // Stop monitoring device status
            StopDeviceStatusMonitoring();
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to disable device. Error: {ex.Message}");
            ErrorOccured?.Invoke(this, ex.Message);
            return false;
        }
        return true;
    }

    public async Task<bool> EnableAsync()
    {
        if (localDevEnvOptions?.Enabled == true)
        {
            logger.LogInformation("::: Local development environment detected. Enabling device in Dev mode :::");
            isInitialized = true;
            isEnabled = true;
            isConnected = true;
            currentStatus = CctalkDeviceStatus.Ok;
            await Task.Delay(500); // Simulate some delay
            return true;
        }

        // Start monitoring device status
        StartDeviceStatusMonitoring();

        if (this.isEnabled)
        {
            logger.LogInformation("Device is already enabled.");
            return true;
        }

        // if (this.IsInhibiting == true)
        // {
        //     this.IsInhibiting = false;
        //     logger.LogInformation("Inhibiting is set to false. Device will be enabled.");
        // }

        try
        {
            ModifyInhibitStatus((int)inhibitMask, (int)inhibitMask >> 8);

            // Ensure polling is stopped before starting again
            if (IsPolling)
            {
                logger.LogInformation("Polling still active during enable. Stopping first.");
                EndPoll();
                await Task.Delay(100);
            }

            StartPoll();
            this.isEnabled = true;
            DeviceEnabled?.Invoke(this, null);
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to enable device. Error: {ex.Message}");
            this.isEnabled = false;
            DeviceDisabled?.Invoke(this, null);

            this.deviceError = "Failed to enable device: " + ex.Message;
            ErrorOccured?.Invoke(this, ex.Message);
            if (IsPolling)
            {
                EndPoll();
            }
            StopDeviceStatusMonitoring();
            return false;
        }
        logger.LogInformation("Device enabled successfully.");
        return true;
    }

    public string GetAdditionalDeviceInfo()
    {
        string info = $"Name: {this.Name}  " + Environment.NewLine +
                      $"Port: {this.portConfiguration?.PortName}  " + Environment.NewLine +
                      $"Baud Rate: {this.portConfiguration?.BaudRate}  " + Environment.NewLine +
                      $"Device Serial number: {this.SerialNumber}" + Environment.NewLine +
                      $"Manufacturer: {this.Manufacturer}" + Environment.NewLine +
                      $"Device category: {this.DeviceCategory}" + Environment.NewLine +
                      $"Coin Configuration: " + Environment.NewLine;

        foreach (var coin in this._coins)
        {
            info += $"  - Coin Code: {coin.Key}, Name: {coin.Value.Name}, Value: {coin.Value.Value}, Enabled: {coin.Value.IsEnabled}" + Environment.NewLine;
        }
        //$"Firmware Version: {fwVersion} " + Environment.NewLine;

        return info;
    }

    public async Task<string> GetCurrentStatus()
    {
        if (localDevEnvOptions?.Enabled == true)
        {
            return "OK (Dev Mode)";
        }
        return Enum.GetName(typeof(CctalkDeviceStatus), currentStatus);
    }

    public string GetError()
    {
        if (latestStatus == CctalkDeviceStatus.Ok && Mode != TerminalOperatingMode.Operator)
        {
            // Clear error if device status is OK
            this.deviceError = "";
        }
        return this.deviceError;
    }

    public async Task<string> GetFirmwareVersionAsync()
    {
        try
        {
            return base.CmdRequestSoftwareRevision();
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to get firmware version. Error: {ex.Message}");
            return "Firmware version not available.";
        }
    }

    public string GetWarning()
    {
        return this.deviceWarning;
    }

    public async Task<bool> InitializeAsync()
    {
        if (localDevEnvOptions?.Enabled == true)
        {
            logger.LogInformation("::: Local development environment detected. Initializing device in Dev mode :::");
            isInitialized = true;
            isEnabled = true;
            isConnected = true;
            currentStatus = CctalkDeviceStatus.Ok;
            return true;
        }

        try
        {
            if (portConfiguration == null)
            {
                throw new InvalidOperationException("Port configuration is not set.");
            }

            // Open the serial port
            if (!IsSerialPortConnected)
            {
                var status = await this.OpenConnectionAsync(portConfiguration);
                if (!status)
                {
                    logger.LogError("Failed to open serial port connection");
                    return false;
                }
            }

            base.Address = (byte)CctalkDeviceTypes.CoinAcceptor;

            // Configure coin denominations
            try
            {
                if (this.configuration?.CoinDenominationConfig == null || !this.configuration.CoinDenominationConfig.Any())
                {
                    throw new InvalidOperationException("Coin configuration is null or empty.");
                }

                foreach (var coin in this.configuration.CoinDenominationConfig)
                {
                    var coinInfo = new CoinTypeInfo($"{coin.Value} {coin.Currency}", coin.Value);
                    this._coins[coin.Code] = coinInfo;

                    logger.LogInformation($"Configured coin: Code={coin.Code}, Value={coin.Value} {coin.Currency}, Enabled={coin.IsEnabled}");
                }

                // Create and apply inhibit mask based on configuration
                inhibitMask = CreateInhibitMaskFromConfiguration(this.configuration.CoinDenominationConfig);
                ModifyInhibitStatus((int)inhibitMask, (int)inhibitMask >> 8);

                logger.LogInformation($"Applied coin inhibit mask: {inhibitMask} (0x{(int)inhibitMask:X4})");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to configure coin denominations: {ex.Message}");
                throw new InvalidOperationException("Coin configuration failed", ex);
            }

            // Configure error mappings
            try
            {
                Dictionary<byte, string> errorNames = Enum.GetValues(typeof(CoinAcceptorErrors))
                                                      .Cast<CoinAcceptorErrors>()
                                                      .ToDictionary(e => (byte)e, e => e.ToString());
                foreach (var error in errorNames)
                {
                    this._errors[error.Key] = error.Value;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to configure error mappings: {ex.Message}");
                // throw new InvalidOperationException("Error mapping configuration failed", ex);
            }

            // Initialize device events - register only once to prevent duplicate handlers
            if (!_eventsRegistered)
            {
                base._CoinAccepted += OnCoinAccepted;
                base.ErrorMessageAccepted += OnErrMsgAccepted;
                _eventsRegistered = true;
                logger.LogDebug("Device events registered.");
            }

            try
            {
                this.Init();
                isInitialized = true;
                isConnected = true;

                await EnableAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to initialize device: {ex.Message}");
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Device initialization failed: {ex.Message}");
            this.deviceError = "Initialization failed: " + ex.Message;
            isInitialized = false;
            isConnected = false;
            return false;
        }
    }

    public async Task<bool> ResetAsync()
    {
        bool ret = false;
        if (localDevEnvOptions?.Enabled == true)
        {
            logger.LogInformation("Device reset in Local developement mode");
            isInitialized = true;
            isEnabled = true;
            isConnected = true;
            currentStatus = CctalkDeviceStatus.Ok;
            await Task.Delay(500); // Simulate some delay
            return true;
        }

        deviceError = string.Empty; // Clear previous error
        deviceWarning = string.Empty; // Clear previous warning

        // Perform reset
        // check for connection, if not connected try to open serial port
        if (IsConnected == false)
        {
            // just perform new re-initialization, it will open serial port if not opened
            try
            {
                ret = await InitializeAsync();
                await Task.Delay(500); // Give some time after initialization
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to re-initialize device during reset. Error: {ex.Message}");
                deviceError = "Failed to re-initialize device during reset: " + ex.Message;
                ret = false;
            }
        }
        else
        {
            try
            {
                ret = this.CmdReset();
                await Task.Delay(500); // Give some time after reset
                logger.LogInformation("Device reset successfully.");
                inhibitMask = CreateInhibitMaskFromConfiguration(this.configuration.CoinDenominationConfig);
                ModifyInhibitStatus((int)inhibitMask, (int)inhibitMask >> 8); // Re-apply inhibit status after reset
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to reset device. Error: {ex.Message}");
                deviceError = "Failed to reset device: " + ex.Message;
                ret = false;
            }
        }
        return ret;
    }

    public async Task<bool> ResetAsync(IBasicHardwareDeviceConfiguration configuration)
    {
        if (configuration is CoinAcceptorRM5HDConfiguration coinAcceptorConfiguration)
        {
            this.configuration = coinAcceptorConfiguration;
            return await ResetAsync();
        }
        throw new ArgumentException("Invalid configuration type. Expected CoinAcceptorConfiguration.");
    }

    public TerminalOperatingMode Mode { get; private set; }

    public void SetOperatingMode(TerminalOperatingMode mode)
    {
        Mode = mode;

        // Skip hardware communication in local dev mode
        if (localDevEnvOptions?.Enabled == true)
        {
            logger.LogInformation($"SetOperatingMode in LocalDev mode: {mode}");
            return;
        }

        if (mode == TerminalOperatingMode.Operator)
        {
            // Only attempt hardware operations if properly connected
            if (isConnected && !reconnectionInProgress)
            {
                try
                {
                    this.IsInhibiting = true; // Disable device for accepting coins in operator mode
                }
                catch (DeviceCommunicationException ex)
                {
                    // Log error but don't let it crash the application
                    logger.LogError($"Failed to set inhibit mode - device communication error: {ex.Message}");
                    deviceError = "Communication error: Unable to set device mode";

                    // Mark device as disconnected to trigger reconnection on next check
                    isConnected = false;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Failed to set inhibit mode: {ex.Message}");
                    deviceError = $"Error setting device mode: {ex.Message}";
                }
            }
        }
    }

    private async Task PerformPeriodicDeviceCheckAsync()
    {
        if (reconnectionInProgress || _isDisposed)
        {
            // Skip status check if reconnection is in progress
            return;
        }

        try
        {
            if (isConnected && isEnabled)
            {
                await deviceAccessSemaphore.WaitAsync();
                try
                {
                    var status = GetStatus();

                    // Update status and handle state changes
                    var previousStatus = latestStatus;
                    latestStatus = status;

                    if (status == CctalkDeviceStatus.OtherError)
                    {
                        // No response or communication error -> Device disconnected
                        consecutiveErrorCount++;
                        if (consecutiveErrorCount >= MAX_CONSECUTIVE_ERRORS_FOR_RECONECT)
                        {
                            var reconnected = await DeviceReconnectLoop();
                            if (reconnected == false)
                            {
                                // Unsuccessful reconnection
                                deviceStatus = "Device disconnected";
                                StopDeviceStatusMonitoring();
                                return;
                            }
                        }
                    }
                    else
                    {
                        // Handle operating mode restrictions
                        if (Mode == TerminalOperatingMode.Operator && !this.IsInhibiting)
                        {
                            this.IsInhibiting = true; // Disable device in operator mode
                        }
                        else if (Mode != TerminalOperatingMode.Operator && this.isEnabled)
                        {
                            this.IsInhibiting = false; // Enable device in user mode
                        }

                        // Reset error counter on successful status check
                        consecutiveErrorCount = 0;
                    }
                }
                catch (DeviceCommunicationException)
                {
                    consecutiveErrorCount++;
                    if (consecutiveErrorCount >= MAX_CONSECUTIVE_ERRORS_FOR_RECONECT)
                    {
                        var reconnected = await DeviceReconnectLoop();
                        if (reconnected == false)
                        {
                            // Unsuccessful reconnection
                            deviceStatus = "Device disconnected";
                            StopDeviceStatusMonitoring();
                            return;
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogError($"Device communication error: {ex.Message}");
                    await DeviceReconnectLoop();
                }
                catch (Exception ex)
                {
                    logger.LogError($"Unexpected error during status check: {ex.Message}");
                }
                finally
                {
                    deviceAccessSemaphore.Release();
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"(unhandled) exception in periodic device check: {ex.Message}");
        }
    }

    private async Task<bool> ReopenSerialInternaly()
    {
        try
        {
            if (portConfiguration == null)
            {
                logger.LogError("Port configuration is null");
                return false;
            }

            var connectionOpened = await this.OpenConnectionAsync(portConfiguration);
            if (!connectionOpened)
            {
                logger.LogError("Failed to reopen serial port");
                return false;
            }

            logger.LogInformation("Serial port successfully reopened");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError($"Error reopening serial port: {ex.Message}");
            return false;
        }
        finally
        {
            await Task.Delay(500); // Delay between attempts and delay after successful open
        }
    }

    private async Task<bool> DeviceReconnectLoop()
    {
        if (reconnectionInProgress || _isDisposed)
        {
            return false;
        }

        DeviceDisconnected?.Invoke(this, EventArgs.Empty);
        try
        {
            if (base.IsPolling)
            {
                logger.LogInformation("Stopping polling due to device disconnection");
                reactivatePollingAfterReconnect = true;
                base.EndPoll();
            }

            isConnected = false;
            reconnectionInProgress = true;
            deviceError = "Device disconnected - Reconnecting . . .";
            deviceWarning = "";

            if (isEnabled)
            {
                logger.LogError("Device disconnected, attempting to reconnect...");

                while (isEnabled && !_isDisposed)
                {
                    if (!IsSerialPortConnected)
                    {
                        if (!await ReopenSerialInternaly())
                        {
                            if (_isDisposed || !isEnabled) break;
                            continue;
                        }
                    }

                    try
                    {
                        // Test communication by attempting to get status
                        var status = GetStatus();
                        if (status != CctalkDeviceStatus.OtherError)
                        {
                            break; // Communication restored
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning($"Reconnection attempt failed: {ex.Message}");
                        await Task.Delay(500);
                    }

                    if (!isEnabled)
                    {
                        reconnectionInProgress = false;
                        logger.LogInformation("Device disabled during reconnection attempts, stopping reconnection.");
                        deviceError = ""; // Clear error because device is disabled
                        deviceWarning = ""; // Clear warning because device is disabled
                        return false;
                    }
                }

                if (!_isDisposed && isEnabled)
                {
                    isConnected = true;
                    reconnectionInProgress = false;
                    logger.LogInformation("Device successfully reconnected");
                    if (reactivatePollingAfterReconnect)
                    {
                        logger.LogInformation("Reactivating polling after reconnection");
                        base.StartPoll();
                        reactivatePollingAfterReconnect = false;
                    }
                    deviceError = ""; // Clear current error
                    deviceWarning = ""; // Clear current warning
                    return true;
                }
                else
                {
                    logger.LogInformation("Device disabled or disposed during reconnection");
                    return false;
                }
            }
            else
            {
                logger.LogError("Device is disabled, cannot attempt reconnection");
                reconnectionInProgress = false;
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Unexpected error during reconnection: {ex.Message}");
            reconnectionInProgress = false;
            return false;
        }
    }

    public void StartDeviceStatusMonitoring()
    {
        statusCheckTimer?.Change(TimeSpan.Zero, STATUS_CHECK_INTERVAL);
        logger.LogInformation("Device status monitoring started");
    }

    public void StopDeviceStatusMonitoring()
    {
        statusCheckTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        logger.LogInformation("Device status monitoring stopped");
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing && !_isDisposed)
        {
            _isDisposed = true;
            isEnabled = false;

            try
            {
                StopDeviceStatusMonitoring();
                statusCheckTimer?.Dispose();

                // Unregister base class events
                if (_eventsRegistered)
                {
                    base._CoinAccepted -= OnCoinAccepted;
                    base.ErrorMessageAccepted -= OnErrMsgAccepted;
                    _eventsRegistered = false;
                    logger?.LogDebug("Device events unregistered.");
                }

                if (deviceAccessSemaphore.Wait(TimeSpan.FromSeconds(2)))
                {
                    deviceAccessSemaphore.Release();
                }

                deviceAccessSemaphore?.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during disposal: {ex.Message}");
            }
        }
        base.Dispose(disposing);
    }

    private CoinIndex CreateInhibitMaskFromConfiguration(List<SingleAcceptorCoinDenomination>? coinConfiguration)
    {
        CoinIndex mask = CoinIndex.None;

        foreach (var coinConfig in coinConfiguration)
        {
            byte bitPosition = coinConfig.Code;

            // Add enabled coins to the mask
            if (coinConfig.IsEnabled)
            {
                // Convert bit position to corresponding CoinIndex flag
                CoinIndex bitFlag = (CoinIndex)(1 << (bitPosition - 1));
                mask |= bitFlag;

                logger?.LogDebug($"Enabling coin: {coinConfig.Value} {coinConfig.Currency} (bit position {bitPosition}, flag: {bitFlag})");
            }
            else
            {
                logger?.LogInformation($"Inhibiting coin: {coinConfig.Value} {coinConfig.Currency} (bit position {bitPosition})");
            }
        }

        logger?.LogDebug($"Created inhibit mask: {mask} (0x{(int)mask:X4})");
        return mask;
    }

    public async Task<OperationResult> RunDiagnosticsCommand(DeviceDiagnosticsCommand command, params object[] args)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command), "Command cannot be null.");
        }

        if (SupportedDiagnosticCommands.Any(c => c.Code == command.Code) == false)
        {
            throw new NotSupportedException($"Command {command.Code} is not supported by the Coin Acceptor Driver.");
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
