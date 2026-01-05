using System.IO.Ports;
using CashVault.DeviceDriver.Common;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using CashVault.ParcelLockerDriver.Messages.RequestMessages;
using CashVault.ParcelLockerDriver.Messages.ResponseMessages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CashVault.ParcelLockerDriver.Config;

namespace CashVault.ParcelLockerDriver;

/// <summary>
/// Parcel Locker driver implementation for serial communication with parcel locker control board
/// </summary>
public class ParcelLockerDriver : BaseSerialPortDriver, IParcelLocker
{
    private readonly IServiceProvider _serviceProvider;
    private ParcelLockerConfiguration _deviceConfiguration;

    public string Name => "CashVault.ParcelLockerDriver";
    public TerminalOperatingMode Mode { get; private set; }

    // Device state
    private bool _isInitialized = false;
    private bool _isConnected = false;
    private bool _isEnabled = false;
    private bool _reconnectionInProgress = false;

    public bool IsInitialized => _isInitialized;
    public bool IsConnected => _isConnected;
    public bool IsEnabled => _isEnabled;
    public bool IsActive => _isConnected && _isEnabled;
    public bool CommandInProgress { get; private set; } = false;

    // Synchronization
    private readonly SemaphoreSlim _deviceAccessSemaphore = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim _statusUpdateSemaphore = new SemaphoreSlim(1, 1);
    private Timer? _statusCheckTimer;
    private readonly TimeSpan _statusCheckInterval = TimeSpan.FromSeconds(5);

    // Status tracking
    private string _currentStatus = "Unknown";
    private string _warnings = "";
    private string _errors = "";
    private byte _boardAddress = 0x01; // Default board address

    // Events
    public event EventHandler? DeviceDisabled;
    public event EventHandler? DeviceEnabled;
    public event EventHandler<string>? ErrorOccured;
    public new event EventHandler? DeviceDisconnected; // Use 'new' to hide base class event
    public event EventHandler<string>? WarningRaised;

    // Diagnostics commands
    private const string RESET_COMMAND = "Reset";
    private const string ENABLE_COMMAND = "Enable";
    private const string DISABLE_COMMAND = "Disable";

    public IEnumerable<DeviceDiagnosticsCommand> SupportedDiagnosticCommands =>
    [
        new DeviceDiagnosticsCommand(RESET_COMMAND, "Reset the parcel locker"),
        new DeviceDiagnosticsCommand(ENABLE_COMMAND, "Enable the parcel locker"),
        new DeviceDiagnosticsCommand(DISABLE_COMMAND, "Disable the parcel locker"),
    ];

    public ParcelLockerDriver(
        Port port,
        ParcelLockerConfiguration configuration,
        IServiceProvider serviceProvider,
        LocalDevEnvOptions? localDevEnvOptions = null)
        : base(serviceProvider.GetRequiredService<ILogger<ParcelLockerDriver>>(), localDevEnvOptions)
    {
        _serviceProvider = serviceProvider;

        if (port.PortType != PortType.Serial)
        {
            throw new ArgumentException("Invalid port type for the parcel locker driver.");
        }

        _deviceConfiguration = configuration;
        _boardAddress = configuration.BoardAddress;

        // Configure serial port based on datasheet specifications
        // Default: 9600 baud, 8 data bits, no parity, 1 stop bit
        this.portConfiguration = new SerialPortConfiguration
        {
            PortName = port.SystemPortName,
            BaudRate = configuration.BaudRate,
            DataBits = 8,
            Parity = Parity.None,
            StopBits = StopBits.One,
            ReadTimeout = 2000,
            WriteTimeout = 2000
        };

        InitializeStatusCheckTimer();
    }

    public void SetOperatingMode(TerminalOperatingMode mode)
    {
        Mode = mode;
    }

    private void InitializeStatusCheckTimer()
    {
        _statusCheckTimer = new Timer(
            async _ => await PerformPeriodicDeviceCheckAsync(),
            null,
            Timeout.Infinite,
            Timeout.Infinite);
    }

    public async Task<bool> InitializeAsync()
    {
        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            CommandInProgress = true;
            logger.LogInformation("Initializing Parcel Locker driver...");

            // Open serial port
            if (!await OpenConnectionAsync(portConfiguration))
            {
                logger.LogError("Failed to open serial port");
                return false;
            }

            _isConnected = true;

            // Query board address to verify communication
            try
            {
                var addressRequest = new QueryBoardAddressRequest();
                var response = await SendRequestAndWaitForResponseAsync<QueryBoardAddressResponse>(addressRequest, 2000);

                if (response != null && response.IsValidMessage())
                {
                    _boardAddress = response.BoardAddress;
                    logger.LogInformation($"Parcel Locker initialized successfully. Board address: {_boardAddress}");
                    _isInitialized = true;
                    _currentStatus = "Initialized";
                    return true;
                }
                else
                {
                    logger.LogWarning("Failed to query board address, using default address");
                    _isInitialized = true;
                    _currentStatus = "Initialized (default address)";
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error querying board address");
                _isInitialized = true;
                _currentStatus = "Initialized (communication error)";
                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Parcel Locker driver");
            _errors = ex.Message;
            return false;
        }
        finally
        {
            CommandInProgress = false;
            _deviceAccessSemaphore.Release();
        }
    }

    public async Task<bool> EnableAsync()
    {
        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            CommandInProgress = true;
            logger.LogInformation("Enabling Parcel Locker...");

            if (!_isConnected)
            {
                logger.LogWarning("Cannot enable: device not connected");
                return false;
            }

            _isEnabled = true;
            _currentStatus = "Enabled";
            DeviceEnabled?.Invoke(this, EventArgs.Empty);

            // Start periodic status check
            _statusCheckTimer?.Change(_statusCheckInterval, _statusCheckInterval);

            logger.LogInformation("Parcel Locker enabled successfully");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to enable Parcel Locker");
            _errors = ex.Message;
            return false;
        }
        finally
        {
            CommandInProgress = false;
            _deviceAccessSemaphore.Release();
        }
    }

    public async Task<bool> DisableAsync()
    {
        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            CommandInProgress = true;
            logger.LogInformation("Disabling Parcel Locker...");

            _isEnabled = false;
            _currentStatus = "Disabled";

            // Stop periodic status check
            _statusCheckTimer?.Change(Timeout.Infinite, Timeout.Infinite);

            DeviceDisabled?.Invoke(this, EventArgs.Empty);

            logger.LogInformation("Parcel Locker disabled successfully");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to disable Parcel Locker");
            return false;
        }
        finally
        {
            CommandInProgress = false;
            _deviceAccessSemaphore.Release();
        }
    }

    public async Task<bool> ResetAsync()
    {
        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            CommandInProgress = true;
            logger.LogInformation("Resetting Parcel Locker...");

            await DisableAsync();
            await CloseConnectionAsync();
            _isConnected = false;
            _isInitialized = false;

            await Task.Delay(1000); // Wait before reinitializing

            var initResult = await InitializeAsync();
            if (initResult)
            {
                await EnableAsync();
            }

            return initResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to reset Parcel Locker");
            return false;
        }
        finally
        {
            CommandInProgress = false;
            _deviceAccessSemaphore.Release();
        }
    }

    public async Task<bool> ResetAsync(IBasicHardwareDeviceConfiguration configuration)
    {
        if (configuration is ParcelLockerConfiguration plConfig)
        {
            _deviceConfiguration = plConfig;
            _boardAddress = plConfig.BoardAddress;
        }
        return await ResetAsync();
    }

    public async Task<bool> OpenCabinetAsync(int cabinetNumber)
    {
        if (!IsActive)
        {
            logger.LogWarning("Cannot open cabinet: device not active");
            return false;
        }

        if (cabinetNumber < 1 || cabinetNumber > 24)
        {
            throw new ArgumentOutOfRangeException(nameof(cabinetNumber), "Cabinet number must be between 1 and 24");
        }

        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            logger.LogInformation($"Opening cabinet {cabinetNumber}...");

            var request = new OpenCabinetRequest(_boardAddress, (byte)cabinetNumber);
            var response = await SendRequestAndWaitForResponseAsync<OpenCabinetResponse>(request, 2000);

            if (response != null && response.IsValidMessage())
            {
                logger.LogInformation($"Cabinet {cabinetNumber} opened successfully");
                return true;
            }

            logger.LogWarning($"Failed to open cabinet {cabinetNumber}");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error opening cabinet {cabinetNumber}");
            return false;
        }
        finally
        {
            _deviceAccessSemaphore.Release();
        }
    }

    public async Task<bool> IsCabinetClosedAsync(int cabinetNumber)
    {
        if (!IsActive)
        {
            logger.LogWarning("Cannot check cabinet status: device not active");
            return false;
        }

        if (cabinetNumber < 1 || cabinetNumber > 24)
        {
            throw new ArgumentOutOfRangeException(nameof(cabinetNumber), "Cabinet number must be between 1 and 24");
        }

        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            var request = new ReadDoorStatusRequest(_boardAddress, (byte)cabinetNumber);
            var response = await SendRequestAndWaitForResponseAsync<ReadSingleDoorStatusResponse>(request, 1000);

            if (response != null && response.IsValidMessage())
            {
                return !response.IsOpen; // Return true if closed
            }

            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error checking cabinet {cabinetNumber} door status");
            return false;
        }
        finally
        {
            _deviceAccessSemaphore.Release();
        }
    }

    public async Task<bool> IsCabinetOccupiedAsync(int cabinetNumber)
    {
        if (!IsActive)
        {
            logger.LogWarning("Cannot check cabinet occupancy: device not active");
            return false;
        }

        if (cabinetNumber < 1 || cabinetNumber > 24)
        {
            throw new ArgumentOutOfRangeException(nameof(cabinetNumber), "Cabinet number must be between 1 and 24");
        }

        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            var request = new ReadIRStatusRequest(_boardAddress, (byte)cabinetNumber);
            var response = await SendRequestAndWaitForResponseAsync<ReadSingleIRStatusResponse>(request, 1000);

            if (response != null && response.IsValidMessage())
            {
                return response.IsOccupied;
            }

            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error checking cabinet {cabinetNumber} occupancy status");
            return false;
        }
        finally
        {
            _deviceAccessSemaphore.Release();
        }
    }

    public async Task<Dictionary<int, bool>> GetAllCabinetDoorStatusAsync()
    {
        var result = new Dictionary<int, bool>();

        if (!IsActive)
        {
            logger.LogWarning("Cannot get cabinet statuses: device not active");
            return result;
        }

        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            var request = new ReadDoorStatusRequest(_boardAddress, 0x00); // 0x00 for all cabinets
            var response = await SendRequestAndWaitForResponseAsync<ReadGroupDoorStatusResponse>(request, 1000);

            if (response != null && response.IsValidMessage())
            {
                for (int i = 1; i <= 24; i++)
                {
                    result[i] = response.IsCabinetClosed(i);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all cabinet door statuses");
            return result;
        }
        finally
        {
            _deviceAccessSemaphore.Release();
        }
    }

    public async Task<Dictionary<int, bool>> GetAllCabinetOccupancyStatusAsync()
    {
        var result = new Dictionary<int, bool>();

        if (!IsActive)
        {
            logger.LogWarning("Cannot get cabinet occupancy: device not active");
            return result;
        }

        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            var request = new ReadIRStatusRequest(_boardAddress, 0x00); // 0x00 for all cabinets
            var response = await SendRequestAndWaitForResponseAsync<ReadGroupIRStatusResponse>(request, 1000);

            if (response != null && response.IsValidMessage())
            {
                for (int i = 1; i <= 24; i++)
                {
                    result[i] = !response.IsCabinetUnoccupied(i); // Invert: unoccupied=false means occupied=true
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all cabinet occupancy statuses");
            return result;
        }
        finally
        {
            _deviceAccessSemaphore.Release();
        }
    }

    public async Task<(Dictionary<int, bool> doorStatus, Dictionary<int, bool> occupancyStatus)> GetAllCabinetStatusAsync()
    {
        var doorStatus = new Dictionary<int, bool>();
        var occupancyStatus = new Dictionary<int, bool>();

        if (!IsActive)
        {
            logger.LogWarning("Cannot get cabinet statuses: device not active");
            return (doorStatus, occupancyStatus);
        }

        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            var request = new ReadIRAndDoorStatusRequest(_boardAddress);
            var response = await SendRequestAndWaitForResponseAsync<ReadIRAndDoorStatusResponse>(request, 1000);

            if (response != null && response.IsValidMessage())
            {
                for (int i = 1; i <= 24; i++)
                {
                    doorStatus[i] = response.IsCabinetDoorClosed(i);
                    occupancyStatus[i] = !response.IsCabinetUnoccupied(i); // Invert for consistency
                }
            }

            return (doorStatus, occupancyStatus);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all cabinet statuses");
            return (doorStatus, occupancyStatus);
        }
        finally
        {
            _deviceAccessSemaphore.Release();
        }
    }

    public async Task<byte> QueryBoardAddressAsync()
    {
        await _deviceAccessSemaphore.WaitAsync();
        try
        {
            var request = new QueryBoardAddressRequest();
            var response = await SendRequestAndWaitForResponseAsync<QueryBoardAddressResponse>(request, 1000);

            if (response != null && response.IsValidMessage())
            {
                _boardAddress = response.BoardAddress;
                return _boardAddress;
            }

            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error querying board address");
            return 0;
        }
        finally
        {
            _deviceAccessSemaphore.Release();
        }
    }

    public Task<string> GetCurrentStatus()
    {
        return Task.FromResult(_currentStatus);
    }

    public string GetWarning()
    {
        return _warnings;
    }

    public string GetError()
    {
        return _errors;
    }

    public string GetAdditionalDeviceInfo()
    {
        return $"Board Address: {_boardAddress}, Baud Rate: {_deviceConfiguration.BaudRate}";
    }

    public async Task<OperationResult> RunDiagnosticsCommand(DeviceDiagnosticsCommand command, params object[] args)
    {
        try
        {
            switch (command.Code)
            {
                case RESET_COMMAND:
                    var resetResult = await ResetAsync();
                    return new OperationResult(resetResult, resetResult ? null : "Reset failed");

                case ENABLE_COMMAND:
                    var enableResult = await EnableAsync();
                    return new OperationResult(enableResult, enableResult ? null : "Enable failed");

                case DISABLE_COMMAND:
                    var disableResult = await DisableAsync();
                    return new OperationResult(disableResult, disableResult ? null : "Disable failed");

                default:
                    return new OperationResult(false, $"Unknown command: {command.Code}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error executing diagnostics command: {command.Code}");
            return new OperationResult(false, ex.Message);
        }
    }

    private async Task PerformPeriodicDeviceCheckAsync()
    {
        if (!_isEnabled || _reconnectionInProgress)
            return;

        await _statusUpdateSemaphore.WaitAsync();
        try
        {
            // Perform a simple status check by reading all cabinet statuses
            var (doorStatus, occupancyStatus) = await GetAllCabinetStatusAsync();

            if (doorStatus.Count > 0)
            {
                _currentStatus = $"Active - {doorStatus.Count} cabinets monitored";
            }
            else
            {
                _currentStatus = "Active - Communication issue";
                _warnings = "Unable to read cabinet statuses";
            }
            logger.LogInformation($"Periodic check: {_currentStatus}"); // remove after testing
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during periodic device check");
            _currentStatus = "Error";
            _errors = ex.Message;
        }
        finally
        {
            _statusUpdateSemaphore.Release();
        }
    }

    private Task<T?> SendRequestAndWaitForResponseAsync<T>(BaseRequestMessage request, int timeoutMs) where T : BaseResponseMessage
    {
        try
        {
            byte[] requestBytes = request.GetMessageBytes();
            logger.LogDebug($"Sending request: {BitConverter.ToString(requestBytes)}");

            // Write request using base class method
            bool writeSuccess = WriteMessageBytesToSerialPort(requestBytes);
            if (!writeSuccess)
            {
                logger.LogWarning("Failed to write request to serial port");
                return Task.FromResult<T?>(null);
            }

            // Wait for response - determine expected response length based on response type
            int expectedLength = GetExpectedResponseLength<T>();
            byte[] responseBytes = ReadSerialPortMessageBytes(expectedLength, timeoutMs);

            if (responseBytes == null || responseBytes.Length == 0)
            {
                logger.LogWarning("No response received");
                return Task.FromResult<T?>(null);
            }

            logger.LogDebug($"Received response: {BitConverter.ToString(responseBytes)}");

            // Create response object
            var response = (T?)Activator.CreateInstance(typeof(T), responseBytes);
            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error sending request or receiving response");
            return Task.FromResult<T?>(null);
        }
    }

    private int GetExpectedResponseLength<T>() where T : BaseResponseMessage
    {
        // Return expected length based on response type
        var typeName = typeof(T).Name;

        return typeName switch
        {
            nameof(SetBaudRateResponse) => 5,
            nameof(QueryBoardAddressResponse) => 5,
            nameof(OpenCabinetResponse) => 5,
            nameof(ReadSingleDoorStatusResponse) => 5,
            nameof(ReadGroupDoorStatusResponse) => 8,
            nameof(ReadSingleIRStatusResponse) => 5,
            nameof(ReadGroupIRStatusResponse) => 8,
            nameof(ReadIRAndDoorStatusResponse) => 12,
            _ => -1 // Read all available bytes
        };
    }

    public new void Dispose()
    {
        _statusCheckTimer?.Dispose();
        _deviceAccessSemaphore?.Dispose();
        _statusUpdateSemaphore?.Dispose();
        CloseConnectionAsync().Wait();
    }
}
