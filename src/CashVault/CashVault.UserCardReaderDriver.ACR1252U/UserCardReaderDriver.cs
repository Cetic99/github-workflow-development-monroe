using CashVault.DeviceDriver.Common;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PCSC;
using PCSC.Utils;

namespace CashVault.UserCardReaderDriver.ACR1252U;

public class UserCardReaderDriver : IUserCardReader, IDisposable
{
    private ISCardContext? context;
    private SCardReader? cardReaderInstance;
    private string? cardReaderName;

    // Concurrency
    private readonly SemaphoreSlim deviceAccessSemaphore = new(1, 1);
    private volatile bool suppressEvents = false;

    // Listening
    private CancellationTokenSource? listenTaskCt;
    private Task? listenTask;

    // Reconnect watchdog
    private CancellationTokenSource? reconnectTaskCt;
    private Task? reconnectTask;

    // ICardReader events
    public event EventHandler<(string, Guid)> CardAuthenticated;
    public event EventHandler<string> CardAuthenticationFailed;
    public event EventHandler<string> CardEnrolled;
    public event EventHandler<string> CardEnrollmentFailed;

    // IBasicHardwareDevice events
    public event EventHandler? DeviceDisabled;
    public event EventHandler? DeviceEnabled;
    public event EventHandler? DeviceDisconnected;
    public event EventHandler<string>? ErrorOccured;
    public event EventHandler<string>? WarningRaised;
    private readonly IServiceProvider serviceProvider;

    private const byte START_PAGE = 8;

    // Status
    public string Name => "CashVault.UserCardReaderDriver.ACR1252U";
    public TerminalOperatingMode Mode { get; private set; }
    public void SetOperatingMode(TerminalOperatingMode mode) => Mode = mode;
    private bool localDevEnvEnabled = false;
    private ILogger logger;

    private volatile bool isInitialized = false;
    private volatile bool isEnabled = false;
    private volatile bool isConnected = false;
    private string lastError = string.Empty;
    private string lastWarning = string.Empty;

    public bool IsInitialized => isInitialized;
    public bool IsConnected => isConnected;
    public bool IsEnabled => isEnabled;
    public bool IsActive => IsConnected && IsEnabled;

    // Diagnostics
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

    // Watchdog config
    private const int WATCHDOG_PERIOD_MS = 1500;
    private const int HEALTH_PING_TIMEOUT_MS = 150;

    public UserCardReaderDriver(IServiceProvider serviceProvider, LocalDevEnvOptions? localDevEnvOptions = null)
    {
        this.serviceProvider = serviceProvider;

        var terminal = (ITerminal)serviceProvider.GetService(typeof(ITerminal))!;
        logger = serviceProvider.GetRequiredService<ILogger<UserCardReaderDriver>>();

        this.SetOperatingMode(terminal.OperatingMode);

        if (localDevEnvOptions != null && localDevEnvOptions.Enabled)
        {
            localDevEnvEnabled = localDevEnvOptions.Enabled;
            // In local dev env, we do not attempt to connect to real hardware
            isInitialized = true;
            isConnected = true;
            isEnabled = true;
            logger.LogInformation("::: LocalDevEnv - User Card Reader Driver created. :::");
            return; // skip real hardware init
        }

        // Initial PC/SC
        if (!TryEstablishContextAndReader(out var err))
            throw new InvalidOperationException(err ?? "No available NFC card readers.");

    }

    public async Task<OperationResult> RunDiagnosticsCommand(DeviceDiagnosticsCommand command, params object[] args)
    {
        if (command == null) throw new ArgumentNullException(nameof(command), "Command cannot be null.");

        if (!SupportedDiagnosticCommands.Any(c => c.Code == command.Code))
            throw new NotSupportedException($"Command {command.Code} is not supported by NFC card reader Driver.");

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

    public async Task<bool> InitializeAsync()
    {
        if (localDevEnvEnabled)
        {
            logger.LogInformation("::: LocalDevEnv - User Card Reader Initialize Async. :::");
            isInitialized = true;
            isConnected = true;
            isEnabled = true;
            await Task.Delay(500);
            DeviceEnabled?.Invoke(this, null);
            return await Task.FromResult(true);
        }

        try
        {
            if (isInitialized) return true;

            if (!TryEstablishContextAndReader(out var err))
                throw new InvalidOperationException(err ?? "Reader init failed.");

            isInitialized = true;
            isConnected = true;
            //Temp fix for NFC reader enable
            await EnableAsync();
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            RaiseError($"Initialize failed: {ex.Message}");
            isInitialized = false;
            isConnected = false;
            isEnabled = false;
            return false;
        }
    }

    public async Task<bool> EnableAsync()
    {
        if (localDevEnvEnabled)
        {
            logger.LogInformation("::: LocalDevEnv - User Card Reader Enable Async. :::");
            isInitialized = true;
            isConnected = true;
            isEnabled = true;
            await Task.Delay(500);
            DeviceEnabled?.Invoke(this, null);
            return await Task.FromResult(true);
        }

        try
        {
            if (!isInitialized)
            {
                var ok = await InitializeAsync();
                if (!ok) return false;
            }
            if (isEnabled) return true;

            StartListening();
            StartReconnectWatchdog();

            isEnabled = true;
            RaiseDeviceEnabled();
            return true;
        }
        catch (Exception ex)
        {
            RaiseError($"Enable failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DisableAsync()
    {
        if (localDevEnvEnabled)
        {
            logger.LogInformation("::: LocalDevEnv - User Card Reader Disable Async. :::");
            isEnabled = false;
            await Task.Delay(500);
            DeviceDisabled?.Invoke(this, null);
            return await Task.FromResult(true);
        }

        try
        {
            if (!isEnabled) return true;

            StopReconnectWatchdog();
            StopListening();

            isEnabled = false;
            RaiseDeviceDisabled();
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            RaiseError($"Disable failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ResetAsync()
    {
        if (localDevEnvEnabled)
        {
            logger.LogInformation("::: LocalDevEnv - User Card Reader Reset Async. :::");
            isInitialized = true;
            isConnected = true;
            isEnabled = true;
            await Task.Delay(500);
            DeviceEnabled?.Invoke(this, null);
            return await Task.FromResult(true);
        }

        try
        {
            var wasEnabled = isEnabled;
            if (wasEnabled) await DisableAsync();

            try { cardReaderInstance?.Disconnect(SCardReaderDisposition.Leave); } catch { }
            isConnected = true;

            if (wasEnabled) await EnableAsync();
            return true;
        }
        catch (Exception ex)
        {
            RaiseError($"Reset failed: {ex.Message}");
            return false;
        }
    }

    public Task<bool> ResetAsync(IBasicHardwareDeviceConfiguration configuration) => ResetAsync();

    public Task<string> GetCurrentStatus()
    {
        try
        {
            if (context != null && cardReaderName != null)
            {
                var states = new[] { new SCardReaderState { ReaderName = cardReaderName, CurrentState = SCRState.Unaware } };
                var rc = context.GetStatusChange(HEALTH_PING_TIMEOUT_MS, states);

                if (rc == SCardError.NoService || rc == SCardError.ServiceStopped)
                {
                    TransitionToDisconnected($"PC/SC service stopped ({SCardHelper.StringifyError(rc)}).");
                }
                else if (rc == SCardError.Success || rc == SCardError.Timeout)
                {
                    isConnected = true;
                    lastError = string.Empty;
                    lastWarning = string.Empty;
                }
                else
                {
                    RaiseWarning($"Status check: {SCardHelper.StringifyError(rc)}");
                }
            }
        }
        catch (Exception ex)
        {
            isConnected = false;
            RaiseWarning($"Status check failed: {ex.Message}");
        }

        var status = $"Initialized={isInitialized}, Enabled={isEnabled}, Active={IsActive}, Connected={isConnected}, Reader='{cardReaderName ?? "n/a"}', " +
                     $"LastError='{lastError}', LastWarning='{lastWarning}'";
        return Task.FromResult(status);
    }

    public string GetWarning() => lastWarning;
    public string GetError() => lastError;

    public string GetAdditionalDeviceInfo()
    {
        if (localDevEnvEnabled)
        {
            return "::: LocalDevEnv - User Card Reader Device info... :::";
        }

        try
        {
            using var tmp = ContextFactory.Instance.Establish(SCardScope.System);
            var readers = tmp.GetReaders() ?? Array.Empty<string>();
            return $"PC/SC readers: {string.Join(", ", readers)}";
        }
        catch { return "PC/SC readers: n/a"; }
    }

    private void RaiseDeviceEnabled() => DeviceEnabled?.Invoke(this, EventArgs.Empty);
    private void RaiseDeviceDisabled() => DeviceDisabled?.Invoke(this, EventArgs.Empty);
    private void RaiseDeviceDisconnected() => DeviceDisconnected?.Invoke(this, EventArgs.Empty);
    private void RaiseError(string msg) { lastError = msg; ErrorOccured?.Invoke(this, msg); }
    private void RaiseWarning(string msg) { lastWarning = msg; WarningRaised?.Invoke(this, msg); }

    public bool EnrollCard(IdentificationCard identificationCard)
    {
        try
        {
            if (identificationCard == null)
                throw new ArgumentNullException(nameof(identificationCard), "Identification card cannot be null.");
        }
        catch (Exception ex)
        {
            RaiseError($"EnrollCard failed: {ex.Message}");
            return false;
        }

        try
        {
            if (localDevEnvEnabled)
            {
                logger.LogInformation("::: LocalDevEnv - User Card Reader Enroll Card. :::");
                logger.LogInformation($"Card enrolled: {identificationCard.Guid}, issued by {identificationCard.IssuedBy}, valid from {identificationCard.ValidFrom}, valid to {identificationCard.ValidTo}, remarks: {identificationCard.Remarks}");

                CardEnrolled?.Invoke(this, "CardEnrolled");
                return true;
            }
        }
        catch (Exception ex)
        {
            RaiseError($"EnrollCard failed: {ex.Message}");
            CardEnrollmentFailed?.Invoke(this, ex.Message);
            return false;
        }

        try
        {

            deviceAccessSemaphore.Wait();
            suppressEvents = true;

            try
            {
                WaitForCardPresent(30_000);
                ConnectToReaderOrThrow();

                // Read Card UID
                string cardIdentifier = ReadUidHex();
                if (string.IsNullOrEmpty(cardIdentifier))
                {
                    throw new InvalidOperationException("Failed to read card UID.");
                }

                // Read GUID from user space memory (starting at page 8)
                Guid? cardGuid = ReadGuidFromPages(START_PAGE);
                if (!cardGuid.HasValue)
                {
                    throw new InvalidOperationException("Failed to read GUID from card.");
                }

                // Read serial number from card (pages 4-7, before user space)
                string serialNumber = ReadSerialNumberFromCard();
                if (string.IsNullOrEmpty(serialNumber))
                {
                    throw new InvalidOperationException("Failed to read serial number from card.");
                }

                // Enroll the card with read values
                identificationCard.Enroll(cardIdentifier, cardGuid.Value, serialNumber);

                try { cardReaderInstance?.Disconnect(SCardReaderDisposition.Leave); } catch { }

                CardEnrolled?.Invoke(this, "CardEnrolled");
                return true;
            }
            finally
            {
                suppressEvents = false;
                deviceAccessSemaphore.Release();
            }
        }
        catch (Exception ex)
        {
            try { cardReaderInstance?.Disconnect(SCardReaderDisposition.Leave); } catch { }
            RaiseError($"EnrollCard failed: {ex.Message}");
            CardEnrollmentFailed?.Invoke(this, ex.Message);
            return false;
        }



        //if (localDevEnvEnabled)
        //{
        //    logger.LogInformation("::: LocalDevEnv - User Card Reader Enroll Card. :::");
        //    logger.LogInformation($"Card enrolled: {card.Guid}, issued by {card.IssuedBy}, valid from {card.ValidFrom}, valid to {card.ValidTo}, remarks: {card.Remarks}");

        //    CardEnrolled?.Invoke(this, "CardEnrolled");
        //    return true;
        //}
        //deviceAccessSemaphore.Wait();
        //suppressEvents = true;
        //try
        //{
        //    WaitForCardPresent(30_000);
        //    ConnectToReaderOrThrow();

        //    WriteGuidToPages(card.Guid, START_PAGE);
        //    var back = ReadGuidFromPages(START_PAGE);

        //    if (back.HasValue && back.Value == card.Guid)
        //    {
        //        CardEnrolled?.Invoke(this, "CardEnrolled");
        //        try { cardReaderInstance?.Disconnect(SCardReaderDisposition.Leave); } catch { }
        //        return true;
        //    }

        //    CardEnrollmentFailed?.Invoke(this, "CardEnrollmentFailed");
        //    try { cardReaderInstance?.Disconnect(SCardReaderDisposition.Leave); } catch { }
        //    return false;
        //}
        //catch (Exception ex)
        //{
        //    try { cardReaderInstance?.Disconnect(SCardReaderDisposition.Leave); } catch { }
        //    RaiseError($"EnrollCard failed: {ex.Message}");
        //    CardEnrollmentFailed?.Invoke(this, "CardEnrollmentFailed");
        //    return false;
        //}
        //finally
        //{
        //    suppressEvents = false;
        //    deviceAccessSemaphore.Release();
        //}
    }

    public void StartListening()
    {
        if (listenTask != null) return;
        listenTaskCt = new CancellationTokenSource();
        listenTask = Task.Run(() => ListenLoop(listenTaskCt.Token));
        isEnabled = true;
    }

    public void StopListening()
    {
        try { listenTaskCt?.Cancel(); } catch { }
        try { listenTask?.Wait(500); } catch { }
        listenTask = null;
        listenTaskCt = null;
        isEnabled = false;
    }

    private async Task ListenLoop(CancellationToken ct)
    {
        string? lastUid = null;

        while (!ct.IsCancellationRequested)
        {
            try
            {
                WaitForCardPresent(60_000);

                await deviceAccessSemaphore.WaitAsync(ct);
                try
                {
                    ConnectToReaderOrThrow();

                    var uidHex = ReadUidHex();
                    var guid = ReadGuidFromPages(START_PAGE);

                    if (!suppressEvents && !string.IsNullOrEmpty(uidHex) && uidHex != lastUid)
                    {
                        if (guid.HasValue)
                        {
                            var value = (uidHex, guid.Value);
                            CardAuthenticated?.Invoke(this, value);
                        }
                        else
                            CardAuthenticationFailed?.Invoke(this, "CardAutenticationFailed");

                        lastUid = uidHex;
                    }

                    try { cardReaderInstance?.Disconnect(SCardReaderDisposition.Leave); } catch { }
                }
                finally
                {
                    lastUid = null;
                    deviceAccessSemaphore.Release();
                }

                await WaitForCardAbsentAsync(60_000, ct);
            }
            catch (OperationCanceledException) { break; }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Reader/service is not available."))
            {
                break;
            }
            catch (Exception ex)
            {
                RaiseWarning($"ListenLoop warning: {ex.Message}");
                await Task.Delay(200, ct);
            }
        }
    }

    private void WaitForCardPresent(int timeoutMs)
    {
        if (context == null || cardReaderName == null)
            throw new InvalidOperationException("Reader/service is not available.");

        var states = new[] { new SCardReaderState { ReaderName = cardReaderName, CurrentState = SCRState.Unaware } };
        var t0 = Environment.TickCount;

        while (Environment.TickCount - t0 < timeoutMs)
        {
            var rc = context.GetStatusChange(250, states);
            if (rc == SCardError.NoService || rc == SCardError.ServiceStopped)
            {
                TransitionToDisconnected($"PC/SC service stopped ({SCardHelper.StringifyError(rc)}).");
                throw new InvalidOperationException("Reader/service is not available.");
            }
            if (rc != SCardError.Success && rc != SCardError.Timeout)
                throw new InvalidOperationException($"GetStatusChange: {SCardHelper.StringifyError(rc)}");

            var present = (states[0].EventState & SCRState.Present) == SCRState.Present;
            if (present) return;

            Thread.Sleep(100);
            states[0].CurrentState = states[0].EventState;
        }
    }

    private async Task WaitForCardAbsentAsync(int timeoutMs, CancellationToken ct)
    {
        if (context == null || cardReaderName == null) return;

        var states = new[] { new SCardReaderState { ReaderName = cardReaderName, CurrentState = SCRState.Unaware } };
        var t0 = Environment.TickCount;

        while (Environment.TickCount - t0 < timeoutMs)
        {
            ct.ThrowIfCancellationRequested();
            var rc = context.GetStatusChange(250, states);
            if (rc == SCardError.NoService || rc == SCardError.ServiceStopped)
            {
                TransitionToDisconnected($"PC/SC service stopped ({SCardHelper.StringifyError(rc)}).");
                return;
            }
            if (rc != SCardError.Success && rc != SCardError.Timeout)
                throw new InvalidOperationException($"GetStatusChange: {SCardHelper.StringifyError(rc)}");

            var present = (states[0].EventState & SCRState.Present) == SCRState.Present;
            if (!present) return;

            await Task.Delay(100, ct);
            states[0].CurrentState = states[0].EventState;
        }
    }

    private void ConnectToReaderOrThrow()
    {
        if (cardReaderInstance == null || cardReaderName == null)
            throw new InvalidOperationException("Reader/service is not available.");

        var rc = cardReaderInstance.Connect(cardReaderName, SCardShareMode.Shared, SCardProtocol.Any);
        if (rc == SCardError.NoService || rc == SCardError.ServiceStopped)
        {
            TransitionToDisconnected($"PC/SC service stopped ({SCardHelper.StringifyError(rc)}).");
            throw new InvalidOperationException("Reader/service is not available.");
        }
        if (rc != SCardError.Success)
            throw new InvalidOperationException($"Connect: {SCardHelper.StringifyError(rc)}");
    }

    private byte[]? Transceive(byte[] apdu, out byte[] sw)
    {
        sw = new byte[] { 0x6F, 0x00 };

        if (cardReaderInstance == null)
            return null;

        var sendPci = SCardPCI.GetPci(cardReaderInstance.ActiveProtocol);
        var recv = new byte[258];
        var rc = cardReaderInstance.Transmit(sendPci, apdu, new SCardPCI(), ref recv);

        if (rc == SCardError.NoService || rc == SCardError.ServiceStopped)
        {
            TransitionToDisconnected($"PC/SC service stopped during transmit ({SCardHelper.StringifyError(rc)}).");
            return null;
        }

        if (rc != SCardError.Success || recv.Length < 2)
            return null;

        sw = recv[^2..];
        var len = recv.Length - 2;
        return len > 0 ? recv.Take(len).ToArray() : Array.Empty<byte>();
    }

    private bool TryTransmitOk(byte[] apdu)
    {
        var _ = Transceive(apdu, out var sw);
        return sw[0] == 0x90 && sw[1] == 0x00;
    }

    private static byte[] BuildEscapeApdu(byte[] nativePayload)
    {
        // ACS escape: FF 00 00 00 Lc <payload>
        var apdu = new byte[5 + nativePayload.Length];
        apdu[0] = 0xFF; apdu[1] = 0x00; apdu[2] = 0x00; apdu[3] = 0x00; apdu[4] = (byte)nativePayload.Length;
        Array.Copy(nativePayload, 0, apdu, 5, nativePayload.Length);
        return apdu;
    }

    private static byte[] BuildPcscRead16(byte startPage) => new byte[] { 0xFF, 0xB0, 0x00, startPage, 0x10 };

    private static byte[] BuildPcscWrite4(byte page, ReadOnlySpan<byte> data4)
    {
        if (data4.Length != 4) throw new ArgumentException("BuildPcscWrite4 expects exactly 4 bytes.");
        var apdu = new byte[9] { 0xFF, 0xD6, 0x00, page, 0x04, 0, 0, 0, 0 };
        data4.CopyTo(apdu.AsSpan(5, 4));
        return apdu;
    }

    private static string ToHexString(byte[]? data) => data == null ? "" : BitConverter.ToString(data).Replace("-", ":");

    private string ReadUidHex()
    {
        var getUid = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
        var resp = Transceive(getUid, out var sw);
        return (resp != null && sw[0] == 0x90 && sw[1] == 0x00) ? ToHexString(resp) : "";
    }

    private Guid? ReadGuidFromPages(byte startPage)
    {
        if (context == null) return null;

        var endPage = (byte)(startPage + 3);

        var resp = Transceive(BuildPcscRead16(startPage), out var sw);
        if (sw[0] == 0x90 && sw[1] == 0x00 && resp is { Length: >= 16 })
        {
            var data16 = TakeLast(resp, 16);
            return GuidFromDataOrNull(data16);
        }

        foreach (var payload in new[]
        {
            new byte[] { 0xD4, 0x42, 0x3A, startPage, endPage },
            new byte[] { 0xD4, 0x40, 0x3A, startPage, endPage }
        })
        {
            resp = Transceive(BuildEscapeApdu(payload), out sw);
            if (sw[0] == 0x90 && sw[1] == 0x00 && resp is { Length: >= 16 })
            {
                var unwrapped = UnwrapPn532(resp);
                if (unwrapped.Length >= 16)
                {
                    var data16 = TakeLast(unwrapped, 16);
                    return GuidFromDataOrNull(data16);
                }
            }
        }

        return null;

        static byte[] TakeLast(byte[] src, int n) =>
            src.Skip(Math.Max(0, src.Length - n)).ToArray();

        static byte[] UnwrapPn532(byte[] r)
        {
            if (r.Length >= 3 && r[0] == 0xD5 && (r[1] == 0x41 || r[1] == 0x43))
            {
                if (r[2] == 0x00) return r.Skip(3).ToArray();
                return Array.Empty<byte>();
            }
            return r;
        }

        static Guid? GuidFromDataOrNull(byte[] buf)
        {
            if (buf.Length < 16) return null;
            var g = buf.Take(16).ToArray();
            if (g.All(b => b == 0x00) || g.All(b => b == 0xFF)) return null;
            return new Guid(g);
        }
    }

    private string ReadSerialNumberFromCard()
    {
        try
        {
            // Serial number is 8 bytes stored in pages 4-5
            // Each page contains 4 bytes, so we need pages 4 and 5
            const byte SERIAL_START_PAGE = 4;

            var resp = Transceive(BuildPcscRead16(SERIAL_START_PAGE), out var sw);
            if (sw[0] == 0x90 && sw[1] == 0x00 && resp is { Length: >= 16 })
            {
                var data16 = resp.Skip(Math.Max(0, resp.Length - 16)).Take(16).ToArray();
                // Serial number is in the first 8 bytes (pages 4-5)
                return ParseSerialNumber(data16.Take(8).ToArray());
            }

            // Try escape command with D4 42
            var payload1 = new byte[] { 0xD4, 0x42, 0x3A, SERIAL_START_PAGE, (byte)(SERIAL_START_PAGE + 1) };
            resp = Transceive(BuildEscapeApdu(payload1), out sw);
            if (sw[0] == 0x90 && sw[1] == 0x00 && resp is { Length: >= 8 })
            {
                var unwrapped = UnwrapPn532Response(resp);
                if (unwrapped.Length >= 8)
                {
                    return ParseSerialNumber(unwrapped.Take(8).ToArray());
                }
            }

            // Try escape command with D4 40
            var payload2 = new byte[] { 0xD4, 0x40, 0x3A, SERIAL_START_PAGE, (byte)(SERIAL_START_PAGE + 1) };
            resp = Transceive(BuildEscapeApdu(payload2), out sw);
            if (sw[0] == 0x90 && sw[1] == 0x00 && resp is { Length: >= 8 })
            {
                var unwrapped = UnwrapPn532Response(resp);
                if (unwrapped.Length >= 8)
                {
                    return ParseSerialNumber(unwrapped.Take(8).ToArray());
                }
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            RaiseWarning($"ReadSerialNumber failed: {ex.Message}");
            return string.Empty;
        }
    }

    private string ParseSerialNumber(byte[] data)
    {
        if (data == null || data.Length < 8)
            return string.Empty;

        // Convert 8 bytes to string, assuming ASCII encoding
        // Serial number format: 8 bytes (e.g., "12345678")
        var text = System.Text.Encoding.ASCII.GetString(data).TrimEnd('\0', ' ', (char)0xFF);
        
        // Remove any non-printable characters
        var cleanText = new string(text.Where(c => !char.IsControl(c) && c != 0xFF).ToArray());
        
        return cleanText;
    }

    private byte[] UnwrapPn532Response(byte[] response)
    {
    if (response.Length >= 3 && response[0] == 0xD5 && (response[1] == 0x41 || response[1] == 0x43))
    {
         if (response[2] == 0x00)
            {
      return response.Skip(3).ToArray();
 }
  return Array.Empty<byte>();
        }
  return response;
    }

    private void WriteGuidToPages(Guid guid, byte startPage)
    {
    if (startPage < 8)
   throw new ArgumentOutOfRangeException(nameof(startPage), "Pages 0–7 are not user memory on NTAG215.");

        var data = guid.ToByteArray();
        for (int i = 0; i < 4; i++)
        {
            var page = (byte)(startPage + i);
            var chunk = data.AsSpan(i * 4, 4).ToArray();

         // Escape WRITE (A2) – D4 42
         if (TryTransmitOk(BuildEscapeApdu(new byte[] { 0xD4, 0x42, 0xA2, page, chunk[0], chunk[1], chunk[2], chunk[3] }))) continue;
         // Escape WRITE (A2) – D4 40
         if (TryTransmitOk(BuildEscapeApdu(new byte[] { 0xD4, 0x40, 0xA2, page, chunk[0], chunk[1], chunk[2], chunk[3] }))) continue;
         // Fallback: PC/SC WRITE 4B
         if (TryTransmitOk(BuildPcscWrite4(page, chunk))) continue;

          throw new InvalidOperationException($"Write failed at page {page}, card may be locked.");
        }
    }

    private void TransitionToDisconnected(string reason)
    {
 try { StopListening(); } catch { }
    isEnabled = false;
   isConnected = false;
        RaiseWarning(reason);
        RaiseDeviceDisconnected();
     StartReconnectWatchdog();
    }

    private void StartReconnectWatchdog()
    {
   if (reconnectTask != null) return;
        reconnectTaskCt = new CancellationTokenSource();
        reconnectTask = Task.Run(() => ReconnectLoopAsync(reconnectTaskCt.Token));
    }

    private void StopReconnectWatchdog()
    {
        try { reconnectTaskCt?.Cancel(); } catch { }
        try { reconnectTask?.Wait(500); } catch { }
        reconnectTask = null;
        reconnectTaskCt = null;
    }

    private async Task ReconnectLoopAsync(CancellationToken ct)
    {
  var preferredName = cardReaderName;

     while (!ct.IsCancellationRequested)
        {
        try
  {
       if (IsConnected && isEnabled)
     {
                    await Task.Delay(WATCHDOG_PERIOD_MS, ct);
           continue;
      }

         var ok = await TryReestablishAsync(preferredName, ct);
       if (ok)
            {
      {
 isConnected = true;
     lastError = string.Empty;
        lastWarning = string.Empty;

         if (!isEnabled)
     {
  isEnabled = true;
         StartListening();
                RaiseDeviceEnabled();
      }
          }
    }
     }
     catch (OperationCanceledException) { break; }
  catch (Exception ex)
          {
    RaiseWarning($"Reconnect watchdog: {ex.Message}");
  }

   try { await Task.Delay(WATCHDOG_PERIOD_MS, ct); } catch { break; }
        }
    }

    private async Task<bool> TryReestablishAsync(string? preferredReaderName, CancellationToken ct)
    {
        if (!TryEstablishContextAndReader(out var err, preferredReaderName))
        {
            return false;
        }

        try
    {
 if (context == null || cardReaderName == null) return false;
     var states = new[] { new SCardReaderState { ReaderName = cardReaderName, CurrentState = SCRState.Unaware } };
            var rc = context.GetStatusChange(HEALTH_PING_TIMEOUT_MS, states);

       if (rc == SCardError.NoService || rc == SCardError.ServiceStopped)
                return false;

            if (rc != SCardError.Success && rc != SCardError.Timeout)
          {
       RaiseWarning($"Status after reestablish: {SCardHelper.StringifyError(rc)}");
            }
        }
        catch (Exception ex)
        {
    RaiseWarning($"Status after reestablish failed: {ex.Message}");
        return false;
        }

        isInitialized = true;
        isConnected = true;
        return await Task.FromResult(true);
    }

 private bool TryEstablishContextAndReader(out string? error, string? preferred = null)
    {
        error = null;

        try { cardReaderInstance?.Dispose(); } catch { }
        cardReaderInstance = null;

        try { context?.Dispose(); } catch { }
        context = null;

        try
        {
            context = ContextFactory.Instance.Establish(SCardScope.System);
        }
   catch (Exception ex)
        {
            error = $"PC/SC context establish failed: {ex.Message}";
            return false;
        }

        try
        {
    var readers = context.GetReaders();
       if (readers is null || readers.Length == 0)
     {
          error = "No available NFC card readers.";
  return false;
            }

            cardReaderName = PickReaderName(readers, preferred);

        cardReaderInstance = new SCardReader(context);
   return true;
        }
      catch (Exception ex)
        {
            error = $"Reader init failed: {ex.Message}";
      return false;
      }
    }

    private static string PickReaderName(string[] readers, string? preferred)
  {
      if (!string.IsNullOrWhiteSpace(preferred) && readers.Contains(preferred))
        return preferred;

        var acs = readers.FirstOrDefault(r =>
     r.Contains("ACS", StringComparison.OrdinalIgnoreCase) ||
      r.Contains("ACR", StringComparison.OrdinalIgnoreCase)) ?? readers[0];

      return acs;
    }

    public void Dispose()
    {
 StopReconnectWatchdog();
        StopListening();
        try { cardReaderInstance?.Dispose(); } catch { }
        try { context?.Dispose(); } catch { }
    deviceAccessSemaphore.Dispose();
    }
}
