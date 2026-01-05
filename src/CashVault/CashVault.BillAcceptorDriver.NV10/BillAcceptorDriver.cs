using System.IO.Ports;
using CashVault.BillAcceptorDriver.NV10.Config;
using CashVault.ccTalk.BillAcceptorBase;
using CashVault.ccTalk.ccTalkBase.Devices;
using CashVault.ccTalk.Common.Exceptions;
using CashVault.DeviceDriver.Common;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CashVault.BillAcceptorDriver.NV10;

public class BillAcceptorDriver : CcTalkBillAcceptorBase, IBillAcceptor
{
    private readonly IServiceProvider serviceProvider;
    private new ILogger logger;
    private BillAcceptorNV10Configuration configuration;
    private bool isInitialized = false;
    private bool isConnected = false;
    private bool isEnabled = false;
    private bool _isDisposed = false;

    private volatile string deviceWarning = "";
    private volatile string deviceError = "";
    private volatile string deviceStatus = "Not initialized";
    private BillAcceptorDeviceStatus currentStatus = BillAcceptorDeviceStatus.Unknown;
    private readonly SemaphoreSlim deviceAccessSemaphore = new SemaphoreSlim(1, 1);
    private readonly LocalDevEnvOptions? localDevEnvOptions;
    private bool reEnableAcceptingAfterOperatorMode = false;

    private System.Timers.Timer? _reconnectTimer;
    private const int _RECONNECT_TRY_INTERVAL_MS = 1500; // try reconnect every second
    private bool _eventsRegistered = false; // Track if base class events are registered

    BillIndex inhibitMask = BillIndex.None;

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

    public BillAcceptorDriver(Port port, BillAcceptorNV10Configuration billAcceptorConfiguration, IServiceProvider serviceProvider, LocalDevEnvOptions? localDevEnvOptions = null)
        : base(port, serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        this.configuration = billAcceptorConfiguration;
        logger = serviceProvider.GetRequiredService<ILogger<BillAcceptorDriver>>();
        this.localDevEnvOptions = localDevEnvOptions;

        if (port.PortType != PortType.Serial)
        {
            throw new ArgumentException("BillAcceptorDriver can only be used with Serial ports.");
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

        // Subscribe to device disconnection event
        base.DeviceDisconnectionDetected += OnDeviceDisconnectionDetected;
    }

    public string Name => "CashVault.BillAcceptorDriver.NV10";

    public bool IsInitialized => this.isInitialized;

    public bool IsConnected => this.isConnected;

    public bool IsEnabled => this.isEnabled;

    public bool IsActive => this.isConnected && this.isEnabled;

    public TerminalOperatingMode Mode { get; private set; }

    // IBasicHardwareDevice events
    public event EventHandler? DeviceEnabled;
    public event EventHandler? DeviceDisabled;
    public new event EventHandler? DeviceDisconnected;
    public event EventHandler<string>? ErrorOccured;
    public event EventHandler<string>? WarningRaised;

    // IBillAcceptor events
    public event EventHandler? BillAcceptingStarted;
    public event EventHandler<string>? BillRejected;
    public event EventHandler<decimal>? BillAccepted;
    public event EventHandler<string>? TicketRejected;
    public event EventHandler<string>? TicketAccepted;
    public event EventHandler? StackBoxRemoved;
    public event EventHandler? StackBoxFull;
    public event EventHandler? JamInAcceptor;
    public event EventHandler? JamInStacker;

    private void OnBillAccepted(object? sender, BillAcceptorBillEventArgs e)
    {
        logger.LogInformation($"BILL accepted: {e.BillValue} BAM | Bill Name: {e.BillName} | Bill Code: {e.BillCode}");
        BillAccepted?.Invoke(this, e.BillValue);
    }

    private void OnErrorMessageAccepted(object? sender, BillAcceptorErrorEventArgs e)
    {
        this.deviceError = e.ErrorMessage;
        logger.LogError($"ERROR detected: {e.ErrorMessage}");

        if (e.IsCritical)
        {
            ErrorOccured?.Invoke(this, e.ErrorMessage);
        }
        else
        {
            WarningRaised?.Invoke(this, e.ErrorMessage);
        }
    }

    private void OnWarningRaised(object? sender, string warningMessage)
    {
        this.deviceWarning = warningMessage;
        logger.LogWarning($"WARNING detected: {warningMessage}");
        WarningRaised?.Invoke(this, warningMessage);
    }

    public void SetOperatingMode(TerminalOperatingMode mode)
    {
        Mode = mode;

        // Skip hardware communication in local dev mode
        if (localDevEnvOptions?.Enabled == true)
        {
            logger.LogInformation($"SetOperatingMode in LocalDev mode: {mode}");
            return;
        }

        try
        {
            if (mode == TerminalOperatingMode.Operator)
            {
                // Only attempt hardware operations if properly connected
                if (isConnected && !base.reconnectionInProgress && !IsInhibiting)
                {
                    this.IsInhibiting = true; // Disable device for accepting bills in operator mode
                    reEnableAcceptingAfterOperatorMode = true;
                }
            }
            else
            {
                if (isConnected && !base.reconnectionInProgress && reEnableAcceptingAfterOperatorMode)
                {
                    this.IsInhibiting = false; // Re-enable device for accepting bills
                    reEnableAcceptingAfterOperatorMode = false;
                }
            }
        }
        catch (DeviceCommunicationException ex)
        {
            logger.LogError($"Failed to set inhibit mode - device communication error: {ex.Message}");
            deviceError = "Communication error: Unable to set device mode";
            isConnected = false;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to set inhibit mode: {ex.Message}");
            deviceError = $"Error setting device mode: {ex.Message}";
        }
    }

    public async Task<bool> DisableAsync()
    {
        if (localDevEnvOptions?.Enabled == true)
        {
            logger.LogInformation("::: Local development environment detected. Disabling device in Dev mode :::");
            isInitialized = true;
            isEnabled = false;
            isConnected = true;
            currentStatus = BillAcceptorDeviceStatus.ReadyForAccepting;
            await Task.Delay(500);
            return true;
        }

        try
        {
            IsInhibiting = true;
            logger.LogInformation("Inhibiting is set to true. Device will be disabled.");
            #region TODO
            // TODO: EndPoll() should be replaced with just inhibiting the device
            // Polling can remain active to monitor status
            EndPoll();
            #endregion
            isEnabled = false;
            DeviceDisabled?.Invoke(this, EventArgs.Empty);
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
            currentStatus = BillAcceptorDeviceStatus.ReadyForAccepting;
            await Task.Delay(500);
            return true;
        }

        if (this.isEnabled)
        {
            logger.LogInformation("Device is already enabled.");
            return true;
        }

        try
        {
            StartPoll();
            this.isEnabled = true;

            if (Mode != TerminalOperatingMode.Operator)
            {
                this.IsInhibiting = false; // Enable accepting bills if not in operator mode
            }

            DeviceEnabled?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to enable device. Error: {ex.Message}");
            this.isEnabled = false;
            DeviceDisabled?.Invoke(this, EventArgs.Empty);

            this.deviceError = "Failed to enable device: " + ex.Message;
            ErrorOccured?.Invoke(this, ex.Message);
            if (IsPolling)
            {
                EndPoll();
            }
            return false;
        }
        reEnableAcceptingAfterOperatorMode = true;
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

                      $"Bill Configuration: " + Environment.NewLine;

        foreach (var bill in this._bills)
        {
            info += $"  - Bill Code: {bill.Key}, Name: {bill.Value.Name}, Value: {bill.Value.Value}, Enabled: {bill.Value.IsEnabled}" + Environment.NewLine;
        }

        return info;
    }

    public Task<string> GetCurrentStatus()
    {
        if (localDevEnvOptions?.Enabled == true)
        {
            return Task.FromResult("OK (Dev Mode)");
        }
        return Task.FromResult(Enum.GetName(typeof(CctalkDeviceStatus), currentStatus) ?? "Unknown");
    }

    public string GetError()
    {
        return this.deviceError;
    }

    public Task<string> GetFirmwareVersionAsync()
    {
        try
        {
            return Task.FromResult(base.CmdRequestSoftwareRevision());
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to get firmware version. Error: {ex.Message}");
            return Task.FromResult("Firmware version not available.");
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
            currentStatus = BillAcceptorDeviceStatus.ReadyForAccepting;
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
                await Task.Delay(100); // Short delay to ensure port is ready
            }

            base.Address = (byte)CctalkDeviceTypes.BillValidator;

            // Configure bill denominations
            try
            {
                if (this.configuration?.BillDenominationConfig == null || this.configuration.BillDenominationConfig.Count == 0)
                {
                    throw new InvalidOperationException("Bill configuration is null or empty.");
                }

                // Clear existing bills to prevent duplicates during ResetAsync
                this._bills.Clear();

                foreach (var bill in this.configuration.BillDenominationConfig)
                {
                    // Create BillTypeInfo with IsEnabled flag from configuration
                    var billInfo = new BillTypeInfo(
                        $"{bill.DataValue} {bill.Currency}",
                        bill.DataValue,
                        isEnabled: bill.IsEnabled
                    );

                    this._bills[bill.DataKey] = billInfo;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to configure bill denominations: {ex.Message}");
                throw new InvalidOperationException("Bill configuration failed", ex);
            }

            // Initialize device events - register only once to prevent duplicate handlers
            if (!_eventsRegistered)
            {
                base._BillAccepted += OnBillAccepted;
                base.ErrorMessageAccepted += OnErrorMessageAccepted;
                base.WarningRaised += OnWarningRaised;
                _eventsRegistered = true;
                logger.LogDebug("Device events registered.");
            }

            try
            {
                // perform base initialization
                this.Init();
                base.CmdModifyBillOperatingMode(stacker: false, escrow: false);

                // Create inhibit mask from configuration (based on IsEnabled flag)
                inhibitMask = CreateInhibitingMaskFromBills(this._bills);
                ModifyInhibitStatus((int)inhibitMask, (int)inhibitMask >> 8);

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
            logger.LogInformation("Device reset in Local development mode");
            isInitialized = true;
            isEnabled = true;
            isConnected = true;
            currentStatus = BillAcceptorDeviceStatus.ReadyForAccepting;
            await Task.Delay(500);
            return true;
        }

        if (IsConnected == false)
        {
            try
            {
                deviceError = string.Empty;
                deviceWarning = string.Empty;
                ret = await InitializeAsync();
                await Task.Delay(500);
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
                deviceError = string.Empty;
                deviceWarning = string.Empty;
                await DisableAsync();
                if (this.CmdReset())
                {
                    logger.LogInformation("Device reset command sent successfully.");
                }
                await Task.Delay(2000);
                if (!IsSerialPortConnected)
                {
                    await ReopenSerialInternally();
                    await Task.Delay(500);
                }
                ret = await InitializeAsync();
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
        if (configuration is BillAcceptorNV10Configuration billAcceptorConfiguration)
        {
            this.configuration = billAcceptorConfiguration;
            return await ResetAsync();
        }
        throw new ArgumentException("Invalid configuration type. Expected BillAcceptorConfiguration.");
    }

    private async Task<bool> ReopenSerialInternally()
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
            await Task.Delay(500);
        }
    }

    private void OnDeviceDisconnectionDetected(object? sender, EventArgs e)
    {
        logger.LogDebug("Device disconnection detected from base class, starting reconnection.");

        isConnected = false;
        deviceError = "Device disconnected - Reconnecting . . .";
        deviceWarning = string.Empty;

        // notify uper layers
        DeviceDisconnected?.Invoke(this, EventArgs.Empty);

        // Start reconnect timer (this will trigger reconnection attempts)
        StartReconnectTimer();
    }

    private void StartReconnectTimer()
    {
        if (_reconnectTimer != null)
        {
            logger.LogDebug("Reconnect tier already running..");
            return;
        }

        _reconnectTimer = new System.Timers.Timer(_RECONNECT_TRY_INTERVAL_MS);
        _reconnectTimer.AutoReset = true;
        _reconnectTimer.Elapsed += ReconnectTimerCallback;
        _reconnectTimer.Start();

        logger.LogInformation("Reconnect started.");
    }

    private async void ReconnectTimerCallback(object? sender, System.Timers.ElapsedEventArgs e)
    {
        // Dont attempt recconection if device is disabled or disposed
        if (!isEnabled || _isDisposed)
        {
            StopReconnectTimer();
            return;
        }

        try
        {
            bool success = await TryReconnectOnce();

            if (success)
            {
                logger.LogInformation("Device reconnected successfully, stopping reconnect attempts.");
                StopReconnectTimer();

                // update state
                isConnected = true;
                deviceError = string.Empty;
                deviceWarning = string.Empty;

                try
                {
                    if (Mode != TerminalOperatingMode.Operator)
                    {
                        this.IsInhibiting = false; // Re-enable device for accepting bills
                    }
                    else
                    {
                        this.IsInhibiting = true; // Keep device inhibited in operator mode
                        reEnableAcceptingAfterOperatorMode = true;
                    }
                }
                catch (System.Exception)
                {
                    logger.LogWarning("Failed to set inhibit mode after reconnection.");
                }

                // TODO: Check if initialization or some type of reset is needed here before resuming polling
                // TODO: After reconnection, we need to verify device status
                // - Check if device needs re-initialization
                // - Verify bill configuration is still valid
                // - Consider if device reset is needed
                // - Determine correct device status (ReadyForAccepting vs other states)
                // For now, ResumePollingAfterReconnect() sets status to Unknown
                // and the next successful poll will determine the actual status

                // Resume polling in base class
                base.ResumePollingAfterReconnect();
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug($"Error during reconnection attempt: {ex.Message}");
        }
    }

    private async Task<bool> TryReconnectOnce()
    {
        try
        {
            // Step 1: Reopen serial port if needed
            if (!IsSerialPortConnected)
            {
                if (!await ReopenSerialInternally())
                {
                    return false;
                }
                await Task.Delay(200); // Small delay after opening port
            }

            // Step 2: Test device communication
            var status = base.SimplePoll();

            if (status)
            {
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            logger.LogDebug($"Reconnection attempt failed: {ex.Message}");
            return false;
        }
    }

    private void StopReconnectTimer()
    {
        if (_reconnectTimer != null)
        {
            _reconnectTimer.Elapsed -= ReconnectTimerCallback;
            _reconnectTimer.Stop();
            _reconnectTimer.Dispose();
            _reconnectTimer = null;
            logger.LogInformation("Reconnect timer stopped");
        }
    }

    /// <summary>
    /// Creates inhibiting mask based on _bills dictionary
    /// </summary>
    /// <returns>BillIndex mask for enabled denominations (non-inhibited bills)</returns>
    private BillIndex CreateInhibitingMaskFromBills(Dictionary<byte, BillTypeInfo> bills)
    {
        BillIndex mask = BillIndex.None;

        foreach (var bill in bills)
        {
            byte bitPosition = bill.Key;
            BillTypeInfo billInfo = bill.Value;

            if (billInfo.IsEnabled)
            {
                // Convert bit position to corresponding BillIndex flag
                BillIndex bitFlag = (BillIndex)(1 << (bitPosition - 1));
                mask |= bitFlag;

                logger?.LogInformation($"Enabling bill: {billInfo.Name} (DataKey: {bitPosition}, flag: {bitFlag})");
            }
            else
            {
                logger?.LogInformation($"Inhibiting bill: {billInfo.Name} (DataKey: {bitPosition})");
            }
        }

        logger?.LogInformation($"Created inhibiting mask: {mask} (0x{(int)mask:X4}).");
        return mask;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_isDisposed)
        {
            _isDisposed = true;
            isEnabled = false;

            StopReconnectTimer();

            // Unregister base class events
            if (_eventsRegistered)
            {
                base._BillAccepted -= OnBillAccepted;
                base.ErrorMessageAccepted -= OnErrorMessageAccepted;
                base.WarningRaised -= OnWarningRaised;
                _eventsRegistered = false;
                logger?.LogDebug("Device events unregistered.");
            }

            try
            {
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
