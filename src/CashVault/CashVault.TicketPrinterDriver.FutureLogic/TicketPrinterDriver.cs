using System.IO.Ports;
using System.Text;
using CashVault.DeviceDriver.Common;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.Common;
using CashVault.TicketPrinterDriver.FutureLogic.Commands;
using CashVault.TicketPrinterDriver.FutureLogic.Config;
using CashVault.TicketPrinterDriver.FutureLogic.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CashVault.TicketPrinterDriver.FutureLogic;

public class TicketPrinterDriver : BaseSerialPortDriver, ITITOPrinter
{
    private readonly IServiceProvider serviceProvider;
    public TerminalOperatingMode Mode { get; private set; }

    public void SetOperatingMode(TerminalOperatingMode mode)
    {
        Mode = mode;
    }

    public string Name => "CashVault.TicketPrinterDriver.FutureLogic";
    private TITOPrinterFutureLogicConfiguration ticketPrinterConfiguration;

    private bool isPrinterInitialized = false;
    private bool isPrinterConnected = false;
    private bool isPrinterEnabled = false;
    private bool autoEnable = false; // helper flag for auto enable after resolving critical error

    public event EventHandler TicketPrintingStarted;
    public event EventHandler TicketPrintingCompleted;
    public event EventHandler TicketPrintingFailed;
    public event EventHandler DeviceDisabled;
    public event EventHandler DeviceEnabled;
    public event EventHandler<string> ErrorOccured;
    public event EventHandler DeviceDisconnected;
    public event EventHandler<string> WarningRaised;


    private SemaphoreSlim deviceAccessSemaphore = new SemaphoreSlim(1, 1); // Controls access to the device
    private SemaphoreSlim statusUpdateSemaphore = new SemaphoreSlim(1, 1);
    private Timer statusCheckTimer;
    private readonly TimeSpan _statusCheckInterval = TimeSpan.FromSeconds(1); // Set the check interval
    private bool reconnectionInProgress = false;
    private StatusResponse? latestStatusResponse = null;
    private string _currentStatusString = "Unknown"; // Helper string for GetCurrentStatus()
    private string _warnings = ""; // Helper string for GetWarning()
    private string _errors = ""; // Helper string for GetError()
    private string firmwareVersion = "N/A";

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


    // Initialize the timer to periodically check the device status
    private void InitializeStatusCheckTimer()
    {
        statusCheckTimer = new Timer(async _ => await PerformPeriodicDeviceCheckAsync(), null, Timeout.Infinite, Timeout.Infinite);
    }

    public string GetWarning()
    {
        return this.Warnings;
    }

    public string GetError()
    {
        return this.Errors;
    }

    public TicketPrinterDriver(Port port, TITOPrinterFutureLogicConfiguration titoPrinterConfiguration, IServiceProvider serviceProvider, LocalDevEnvOptions? localDevEnvOptions = null)
        : base(serviceProvider.GetRequiredService<ILogger<TicketPrinterDriver>>(), localDevEnvOptions)
    {
        this.serviceProvider = serviceProvider;

        if (port.PortType != PortType.Serial)
        {
            throw new ArgumentException("Invalid port type for the ticket printer driver.");
        }

        var availableBaudRates = new List<int> { 1200, 2400, 4800, 9600, 19200, 38400 };
        if (availableBaudRates.Contains(titoPrinterConfiguration.BaudRate) == false)
        {
            throw new ArgumentException("Invalid baud rate for the ticket printer driver.");
        }

        this.portConfiguration = new()
        {
            PortName = port.SystemPortName,
            BaudRate = titoPrinterConfiguration.BaudRate,
            DataBits = 8,
            Parity = Parity.None,
            StopBits = StopBits.One,
            WriteTimeout = 2000,
            ReadTimeout = 2000,
        };

        this.ticketPrinterConfiguration = titoPrinterConfiguration;
        InitializeStatusCheckTimer();
        this.WarningRaised = OnWarningRaised;

        var terminal = serviceProvider.GetRequiredService<ITerminal>();
        this.SetOperatingMode(terminal.OperatingMode);

        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            isPrinterInitialized = true;
            isPrinterConnected = true;
            isPrinterEnabled = true;
        }
    }

    private void OnWarningRaised(object sender, string message)
    {
        logger.LogWarning($"Warning: {message}");
    }

    public bool IsInitialized => this.isPrinterInitialized;
    public bool IsConnected => this.isPrinterConnected;
    public bool IsEnabled => this.isPrinterEnabled;
    public bool IsActive => IsConnected && IsEnabled;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            isPrinterEnabled = false;
            statusCheckTimer.Dispose();
            //deviceAccessSemaphore.Dispose();
        }
        base.Dispose(disposing);
    }
    public async Task<bool> DisableAsync()
    {
        if (isPrinterEnabled == false)
        {
            logger.LogInformation("Already disabled.");
        }
        else
        {
            isPrinterEnabled = false;
            this.DeviceDisabled?.Invoke(this, null);
            logger.LogInformation("Disabled successfully.");
        }
        autoEnable = false; // do not enable automaticaly after resolving errors ...
        // Simulate async method
        await Task.Delay(500);
        return true;
    }

    public async Task<bool> EnableAsync()
    {
        if (this.isPrinterEnabled)
        {
            logger.LogInformation("Already enabled.");
        }
        else
        {
            if (!isPrinterInitialized)
            {
                this.isPrinterEnabled = await InitializeAsync();
            }
            else
            {
                this.isPrinterEnabled = true;
                this.DeviceEnabled?.Invoke(this, null);
            }
            logger.LogInformation("Enabled successfully.");
        }
        autoEnable = true;
        return this.isPrinterEnabled;
    }

    public async Task<bool> InitializeAsync()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: Local development environment detected. Printer initialized without connection. :::");
            isPrinterInitialized = true;
            isPrinterConnected = true;
            isPrinterEnabled = true;
            autoEnable = true;
            this.DeviceEnabled?.Invoke(this, null);
            return true;
        }

        if (this.portConfiguration == null)
        {
            throw new InvalidOperationException("Printer port configuration is not set.");
        }

        if (isPrinterConnected == false)
        {
            var connectionOpened = await this.OpenConnectionAsync(this.portConfiguration);
            if (connectionOpened == false)
            {
                logger.LogError($"Printer connection failed.");
                this.DeviceDisconnected?.Invoke(this, null);
                return false;
            }
            else
            {
                // TODO: check connection with device.
                isPrinterConnected = true;
                isPrinterEnabled = true;
                autoEnable = true;
                this.DeviceEnabled?.Invoke(this, null);
            }
        }

        StartDeviceStatusMonitoring();
        isPrinterInitialized = true;
        logger.LogInformation("Printer device initialized.");
        return true;
    }

    private async Task<ISerialPortMessage> GetPrinterStatus()
    {
        if ((isPrinterConnected) ||
            (reconnectionInProgress && isPrinterEnabled))
        {
            // request status
            var response = await this.SendAndReceiveMessageAsync(new GetStatusCommand());

            // response should be statusResponse
            if (response is StatusResponse)
            {
                StatusResponse status = new StatusResponse(response.GetMessageBytes());

                if (status.Errors != null && status.Errors.Any())
                {
                    logger.LogError("Error occured.");
                }
                else
                {
                    logger.LogDebug("Printer status - OK");
                }

                HandleErrors(status);
                HandleStatusInfo(status);
                this.firmwareVersion = status.soft_ver;

                return status;
            }
            else if (response is RAWResponse)
            {
                // TODO: analyze this case... return RAW msg or null ???
                logger.LogError("Wrong response received for GetStatus command.");
                return (RAWResponse)response;
            }
            else
            {
                // no response
                logger.LogError("No response received for GetStatus command.");
                return null;
            }
        }
        else
        {
            logger.LogError("Ticket printer is not enabled/initialized.");
            return null;
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

    private async Task<OperationResult> Print(BaseCommand command)
    {
        //Clearing error status bits
        ClearErrorStatusBits();
        await Task.Delay(100);

        // Check ticket chute
        bool ticketPresent = IsTicketPresentInTicketChute().GetAwaiter().GetResult();

        if (ticketPresent == true)
        {
            TicketPrintingFailed?.Invoke(this, null);
            logger.LogError("Ticket Printing error: Previous ticket not taken.");
            return new OperationResult(false, "Previous ticket not taken.");
        }

        TicketPrintingStarted?.Invoke(this, null);
        ClearErrorStatusBits();
        await Task.Delay(100);
        await this.SendMessageAsync(command);

        // wait for printing. Should not be less than 4000ms
        await Task.Delay(5_000);

        bool ticketTaken = IsTicketTaken(this.ticketPrinterConfiguration.TicketTakingTimeout).GetAwaiter().GetResult();

        if (ticketTaken == true)
        {
            logger.LogInformation("Ticket printed successfully.");
            TicketPrintingCompleted?.Invoke(this, null);
            return new OperationResult(true);
        }
        else
        {
            logger.LogWarning("Ticket printing error occured. Ticket not taken.");
            TicketPrintingFailed?.Invoke(this, null);
            return new OperationResult(false, "Ticket not taken. The time limit for taking the ticket has been exceeded.");
        }
    }

    public async Task<OperationResult> PrintTextAsync(string[] lines)
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: Local development environment detected. Simulate text printing... :::");
            TicketPrintingStarted?.Invoke(this, null);
            await Task.Delay(500); // Simulate printing delay
            TicketPrintingCompleted?.Invoke(this, null);
            return new OperationResult(true);
        }
        deviceAccessSemaphore.WaitAsync().GetAwaiter().GetResult();
        try
        {
            if (isPrinterConnected && isPrinterEnabled)
            {
                var command = new PrintTextCommand(lines);
                OperationResult ret = this.Print(command).GetAwaiter().GetResult();
                return ret;
            }
            else
            {
                logger.LogError("Ticket printer is not initialized.");
                TicketPrintingFailed?.Invoke(this, null);
                return new OperationResult(false, "Ticket printer is not initialized.");
            }
        }
        finally
        {
            deviceAccessSemaphore.Release();
        }
    }

    public async Task<OperationResult> PrintTicketAsync(Ticket ticket, string caption, string locationName, string locationAddress, string machineNumber)
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: Local development environment detected. Simulate ticket printing... :::");
            TicketPrintingStarted?.Invoke(this, null);
            await Task.Delay(500); // Simulate printing delay
            TicketPrintingCompleted?.Invoke(this, null);
            return new OperationResult(true);
        }

        deviceAccessSemaphore.WaitAsync().GetAwaiter().GetResult();
        try
        {
            if (Mode == TerminalOperatingMode.Operator)
            {
                logger.LogError("Terminal is in Operator mode.");
                TicketPrintingFailed?.Invoke(this, null);
                return new OperationResult(false, "Terminal is in Operator mode. Ticket printing not allowed.");
            }
            else if (isPrinterConnected && isPrinterEnabled)
            {
                var command = new PrintTemplate0Command(caption, ticket.Barcode, ticket.Amount, ticket.Currency, locationName, locationAddress, ticket.PrintingRequestedAt, ticket.DaysValid, ticket.Number, machineNumber);

                OperationResult ret = this.Print(command).GetAwaiter().GetResult();
                return ret;
            }
            else
            {
                logger.LogError("Ticket printer is not initialized.");
                TicketPrintingFailed?.Invoke(this, null);
                return new OperationResult(false, "Ticket printer is not initialized.");
            }
        }
        finally
        {
            deviceAccessSemaphore.Release();
        }
    }

    public async Task<bool> ResetAsync()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            logger.LogInformation("::: Local development environment detected. Simulate printer reset... :::");
            await Task.Delay(500); // Simulate reset delay
            logger.LogInformation("Ticket Printer reset done.");
            return true;
        }

        deviceAccessSemaphore.WaitAsync().GetAwaiter().GetResult();
        try
        {
            if (isPrinterConnected && isPrinterEnabled)
            {
                var command = new ResetPrinterCommand();
                await this.SendMessageAsync(command);
                logger.LogInformation("Ticket Printer reset done.");
                return true;
            }
            else
            {
                logger.LogError("Can't execute reset sequence: Printer is not connected/enabled.");
                return false;
            }
        }
        finally
        {
            deviceAccessSemaphore.Release();
        }
    }

    protected override ISerialPortMessage ReadMessageInternal()
    {
        // maximum waiting time for response
        const int WAITING_TIMEOUT_MS = 3000;
        byte[] msgHeader = null;

        try
        {
            // Get first 3 bytes (expecting status header...)
            msgHeader = ReadSerialPortMessageBytes(StatusResponse.MSG_HEADER_LEN, WAITING_TIMEOUT_MS);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while reading Serial port from printer driver. {ex.Message}");
            return null;
        }

        if (msgHeader == null)
        {
            // no response, timeout reached...
            return null;
        }
        else
        {
            // Check if reponse is starting with status response header
            if (Encoding.ASCII.GetString(msgHeader) == StatusResponse.MsgHeader)
            {
                byte[] msgData = null;

                try
                {
                    // Get other bytes from status message (payload)
                    msgData = ReadSerialPortMessageBytes(StatusResponse.STATUS_MSG_LEN - StatusResponse.MSG_HEADER_LEN, WAITING_TIMEOUT_MS);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error while reading Serial port from printer driver. {ex.Message}");
                    return null;
                }

                byte[] statusMsg = null;

                try
                {
                    // create complete byte array of received data
                    statusMsg = msgHeader.Concat(msgData).ToArray();

                    // create and return StatusResponse msg
                    var ret = new StatusResponse(statusMsg);
                    logger.LogDebug($"Status response: {Encoding.ASCII.GetString(statusMsg)} ");
                    return ret;
                }
                catch (ArgumentNullException ex)
                {
                    // should not happen, already covered.. 
                    return null;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Ticket printer response error. Invalid or corrupted message: {ex.Message} ");
                    return new RAWResponse(statusMsg);
                }
            }
            else
            {
                // unexpected message
                // TODO: Flush complete rx buffer...
                // TODO: Implement RAW msg object return
                return null;
            }
        }
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


    private bool HandleErrors(StatusResponse status)
    {
        bool ret = false;
        bool noCriticalErrors = true;

        if (status.Errors != null && status.Errors.Any())
        {
            ret = true;
            foreach (var item in status.Errors)
            {
                string messageTemplate = $"Printer error occurred: " +
                                         $"Code: {item.Code}! " +
                                         $"Description: {item.Description} " +
                                         $"IsCritical: {item.IsCritical}! " +
                                         $"ResetNeeded: {item.ResetNeeded}! " +
                                         $"MaintenanceNeeded: {item.MaintenanceNeeded}! ";

                logger.LogError(messageTemplate);

                if (item.IsCritical)
                {
                    noCriticalErrors = false;
                    if (isPrinterEnabled)
                    {
                        this.isPrinterEnabled = false;
                        this.DeviceDisabled?.Invoke(this, null);
                    }
                }
            }
        }

        if (noCriticalErrors && autoEnable && (!isPrinterEnabled))
        {
            this.isPrinterEnabled = true;
            this.DeviceEnabled?.Invoke(this, null);
        }

        return ret;

    }

    private void HandleStatusInfo(StatusResponse status)
    {
        if (status.InformationStatuses != null && status.InformationStatuses.Any())
        {
            foreach (var item in status.InformationStatuses)
            {
                var allowedCodesForLogger = new List<string> { "SF3_P_PRESENT", "SF5_PWR_RST" }; // just to avoid too much messages..

                string messageTemplate = $"Printer status information: " +
                                         $"Code: {item.Code}. " +
                                         $"Description: {item.Description} ";

                if (allowedCodesForLogger.Contains(item.Code))
                {
                    logger.LogInformation(messageTemplate);
                }
            }
        }
    }

    private async Task PerformPeriodicDeviceCheckAsync()
    {
        if (isPrinterConnected && !reconnectionInProgress)
        {
            deviceAccessSemaphore.WaitAsync().GetAwaiter().GetResult(); // Wait for exclusive access to the device
            try
            {
                logger.LogDebug("Periodic device check...");
                var status = await GetPrinterStatus(); // errors are handled inside GetPrinterStatus() fnc
                HandleStatusChange(latestStatusResponse, status as StatusResponse);
                latestStatusResponse = status as StatusResponse;

                if (status == null)
                {
                    // no response -> Device disconected, go to reconecting
                    await DeviceReconnectLoop();
                }
                else if (status is StatusResponse)
                {
                    ClearErrorStatusBits();
                }
                else
                {
                    // TODO: Handle wrong msg received -> reset, reconnect, disable/enable.... analyze this case !
                    // TODO: Add a counter that counts the number of wrong msg received
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError("Device disconnected!");
                isPrinterConnected = false;
                await DeviceReconnectLoop();
            }
            finally
            {
                deviceAccessSemaphore.Release(); // Release the semaphore after the status check
            }
        }
    }

    // Start the status monitoring
    public void StartDeviceStatusMonitoring()
    {
        statusCheckTimer.Change(TimeSpan.Zero, _statusCheckInterval); // Restart the timer
        logger.LogInformation("Printer status monitoring started.");
    }

    // Stop the status monitoring
    public void StopDeviceStatusMonitoring()
    {
        statusCheckTimer.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer
        logger.LogInformation("Printer status monitoring stopped.");
    }

    private async Task<bool> IsTicketPresentInTicketChute()
    {
        var resp = await GetPrinterStatus();
        StatusResponse? status = resp as StatusResponse;

        if (status == null)
        {
            logger.LogError("Can't get TicketPresent status: No response from device.");
            //throw new Exception("Can't get device status!");
        }
        else if (status.InformationStatuses.Any(item => item.Code == "SF3_P_PRESENT"))
        {
            return true;
        }
        return false;
    }

    private async Task<bool> IsTicketTaken(int maxTimeoutMilliSeconds = 200000)
    {
        if (this.ticketPrinterConfiguration.WaitForTicketTaking == false)
        {
            return true;
        }

        DateTime endTime = DateTime.UtcNow.AddMilliseconds(maxTimeoutMilliSeconds);

        while ((DateTime.UtcNow < endTime))
        {
            if (await IsTicketPresentInTicketChute() == false)
            {
                return true;
            }

            await Task.Delay(500);
        }
        logger.LogDebug($"IsTicketTaken() - the maximum waiting time ({maxTimeoutMilliSeconds}ms) has expired, exiting the function. Ticket not taken.");
        return false;
    }

    private void ClearErrorStatusBits()
    {
        var errClearCmd = new ClearErrorStatusCommand();

        // clear errors
        this.SendMessageAsync(errClearCmd).GetAwaiter().GetResult();
        logger.LogDebug("ErrorStatusBits cleared.");
    }

    private async Task<bool> ReopenSerialInternaly()
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

    private async Task<bool> DeviceReconnectLoop()
    {
        DeviceDisconnected?.Invoke(this, null);
        isPrinterConnected = false;
        reconnectionInProgress = true;

        if (isPrinterEnabled)
        {
            logger.LogError("Device disconnected, trying to reconnect...");
            while (isPrinterEnabled)
            {
                if (IsSerialPortConnected == false)
                {
                    HandleStatusChange(null, null); // update status with disconnection info
                    if (await ReopenSerialInternaly() == false)
                    {
                        continue;
                    }
                }

                try
                {
                    // check comunication with printer
                    if (await GetPrinterStatus() is StatusResponse)
                    {
                        break;
                    }
                }
                catch (ArgumentException ex)
                {
                    // try again
                }
                logger.LogError("Reconnection failed!");
                await Task.Delay(500);
            }

            if (!isPrinterEnabled)
            {
                reconnectionInProgress = false;
                return false;
            }

            isPrinterConnected = true;
            reconnectionInProgress = false;
            logger.LogInformation("Device successfully reconnected.");
            return true;
        }
        else
        {
            logger.LogError("Device disabled. Can't reconnect!");
            reconnectionInProgress = false;
            return false;
        }
    }
    public async Task<string> GetCurrentStatus()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            return "Device status - OK ::: LOCAL DEVELOPEMENT ";
        }
        return this.CurrentStatusString;
    }

    public Task<bool> ResetAsync(IBasicHardwareDeviceConfiguration configuration)
    {
        if (configuration is TITOPrinterFutureLogicConfiguration ticketPrinterConfiguration)
        {
            this.ticketPrinterConfiguration = ticketPrinterConfiguration;
            return this.ResetAsync();
        }

        throw new InvalidOperationException("Invalid configuration type.");
    }

    private void HandleStatusChange(StatusResponse? previousStatus, StatusResponse? newStatus)
    {
        string statusRet = "Device status - OK";
        string errRet = "";
        string wrnRet = "";

        if ((previousStatus == null && newStatus == null))
        {
            if (!isPrinterConnected)
            {
                statusRet = "Device status - ";
                statusRet += reconnectionInProgress ? "Reconnecting!" : "Disconnected!";
                errRet += reconnectionInProgress ? "Reconnecting!" : "Disconnected!";
                CurrentStatusString = statusRet;
                Errors = errRet;
                Warnings = wrnRet;
                logger.LogWarning(statusRet);
            }
            return;
        }

        if ((previousStatus != null && newStatus != null) &&
             (previousStatus.GetMessageBytes().SequenceEqual(newStatus.GetMessageBytes())))
        {
            return;
        }

        if (newStatus == null)
        {
            statusRet = "Error : no response from device!";
            errRet = "Error : no response from device!";
            ErrorOccured?.Invoke(this, errRet);
        }
        else if (newStatus.Errors != null && newStatus.Errors.Any())
        {
            foreach (var error in newStatus.Errors)
            {
                if (error.IsCritical)
                {
                    statusRet = "Device status - Critical error detected!";
                    errRet += (error.Description + "\n");
                    ErrorOccured?.Invoke(this, error.Description);
                }
                else
                {
                    // non-critical error are just warnings
                    WarningRaised?.Invoke(this, error.Description);
                    wrnRet += (error.Description + "\n");
                }
            }
        }
        else
        {
            statusRet = "Device status - OK";
            logger.LogDebug(statusRet);
        }

        if (!isPrinterEnabled)
        {
            statusRet = "Disabled | " + statusRet;
        }

        // Update diagnostic strings
        CurrentStatusString = statusRet;
        Errors = errRet;
        Warnings = wrnRet;
    }

    public string GetAdditionalDeviceInfo()
    {
        if (LocalDevEnvOptions != null && LocalDevEnvOptions.Enabled)
        {
            return "::: Local development environment detected. Ticket Printer Device info... :::";
        }

        string info = $"Name: {this.Name} " + Environment.NewLine +
                      $"Port: {this.portConfiguration?.PortName} " + Environment.NewLine +
                      $"BaudRate: {this.portConfiguration?.BaudRate} " + Environment.NewLine +
                      $"Firmware version: {this.firmwareVersion} " + Environment.NewLine;

        return info;
    }

    private string CurrentStatusString
    {
        get
        {
            statusUpdateSemaphore.Wait();
            try
            {
                return _currentStatusString;
            }
            finally
            {
                statusUpdateSemaphore.Release();
            }
        }
        set
        {
            statusUpdateSemaphore.Wait();
            try
            {
                _currentStatusString = value;
            }
            finally
            {
                statusUpdateSemaphore.Release();
            }
        }
    }

    private string Warnings
    {
        get
        {
            statusUpdateSemaphore.Wait();
            try
            {
                return _warnings;
            }
            finally
            {
                statusUpdateSemaphore.Release();
            }
        }
        set
        {
            statusUpdateSemaphore.Wait();
            try
            {
                _warnings = value;
            }
            finally
            {
                statusUpdateSemaphore.Release();
            }
        }
    }

    private string Errors
    {
        get
        {
            statusUpdateSemaphore.Wait();
            try
            {
                return _errors;
            }
            finally
            {
                statusUpdateSemaphore.Release();
            }
        }
        set
        {
            statusUpdateSemaphore.Wait();
            try
            {
                _errors = value;
            }
            finally
            {
                statusUpdateSemaphore.Release();
            }
        }
    }
}
