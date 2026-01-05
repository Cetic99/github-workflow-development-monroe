using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Timers;
using CashVault.ccTalk.ccTalkBase;
using CashVault.ccTalk.ccTalkBase.Devices;
using CashVault.ccTalk.Common.Exceptions;
using CashVault.Domain.Aggregates.DeviceAggregate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CashVault.ccTalk.BillAcceptorBase
{
    /// <summary>
    /// Encapsulates various routines linked with bill acceptors using ccTalk protocol. 
    /// Also invokes events to Wpf or Windows.Forms application
    /// </summary>
    public abstract class CcTalkBillAcceptorBase : BaseCctalkDevice, IDisposable
    {
        const Int32 PollPeriod = 500;
        const Int32 MaxConsecutiveErrors = 25;

        System.Timers.Timer? _t;
        Byte _lastEvent;
        private int _consecutiveErrorCount;
        protected BillAcceptorDeviceStatus _lastKnownStatus = BillAcceptorDeviceStatus.Unknown;

        // Reconnection handling
        protected bool pausePollingDuringReconnect = false;
        protected bool reconnectionInProgress = false;
        protected event EventHandler? DeviceDisconnectionDetected;


		/// <summary>
		/// Fires when bill was accepted or is in escrow. Only when polling is on.
		/// </summary>
		protected event EventHandler<BillAcceptorBillEventArgs>? _BillAccepted;

        /// <summary>
        /// Fires when any error, fault, or problematic event was detected during poll.
        /// </summary>
        protected event EventHandler<BillAcceptorErrorEventArgs>? ErrorMessageAccepted;
        
        protected event EventHandler<string>? WarningRaised;
        TimeSpan _pollInterval;

        readonly Object _timersSyncRoot = new Object(); // for sync with timer threads only
        protected readonly Dictionary<Byte, BillTypeInfo> _bills = new Dictionary<Byte, BillTypeInfo>();

        private new ILogger? logger;

        public CcTalkBillAcceptorBase(Port port, IServiceProvider serviceProvider) : base(port, serviceProvider)
        {
            logger = serviceProvider.GetRequiredService<ILogger<CcTalkBillAcceptorBase>>();

            // Initialize default bill configuration
            _allowedBills = BillIndex.All;
        }

        public void ModifyAcceptedBills(Dictionary<Byte, BillTypeInfo> bills)
        {
            if (bills != null)
            {
                foreach (var bill in bills)
                {
                    _bills[bill.Key] = bill.Value;
                }
            }
        }

        public void Init(Boolean ignoreLastEvents = true)
        {
            DeviceCategory = base.CmdRequestEquipmentCategory();
            if (DeviceCategory != CctalkDeviceTypes.BillValidator)
            {
                throw new InvalidOperationException("Connected device is not a bill validator. " + DeviceCategory);
            }

            // Initialize in inhibiting mode to prevent accepting bills until ready
            IsInhibiting = true;

            SerialNumber = base.CmdGetSerial();
            PollInterval = base.CmdRequestPollingPriority();
            Manufacturer = base.CmdRequestManufacturerId();
            ProductCode = base.CmdRequestProductCode();

            // Use bill-specific event buffer for initialization
            var evBuf = base.CmdRequestBufferedBillEvents();

            if (!ignoreLastEvents)
            {
                RaiseLastEvents(evBuf);
            }
            _lastEvent = evBuf.Counter;

            isCctalkDevInitialized = true;
        }

        public void ModifyInhibitStatus(int Data1 = 255, int Data2 = 0)
        {
            base.CmdModifyInhibitStatus(Data1, Data2);
        }

        /// <summary>
        ///  Closes port
        /// </summary>
        public void UnInit()
        {
            lock (_timersSyncRoot)
            {
                EndPoll();
                isCctalkDevInitialized = false;
            }
        }

        /// <summary>
        ///  true - port is open, ready for sending commands
        /// </summary>
        public Boolean isCctalkDevInitialized { get; private set; }

        /// <summary>
        ///  Is polling is running now. Commands (as GetStatus) CAN be sent while polling.
        /// </summary>
        public Boolean IsPolling { get { return _t != null; } }

        protected String ProductCode { get; private set; } = string.Empty;

        /// <summary>
        /// Serial number of device. Value accepted from device while Init.
        /// </summary>
        public Int32 SerialNumber { get; private set; }

        /// <summary>
        ///  Manufacter name of device. Value accepted from device while Init.
        /// </summary>
        public String Manufacturer { get; private set; } = string.Empty;

        /// <summary>
        ///  Type of device. Value accepted from device while Init.
        /// </summary>
        public CctalkDeviceTypes DeviceCategory { get; private set; }

        bool _isInhibiting;

        /// <summary>
        ///  Indicates the state, when device is rejecting all bills.
        /// </summary>
        public Boolean IsInhibiting
        {
            get { return _isInhibiting; }
            set
            {
                // Retry logic: Attempt up to 3 times with delays
                const int maxAttempts = 5;
                const int delayMs = 200;
                
                Exception? lastException = null;
                
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        base.CmdSetMasterInhibitStatus(value);
                        _isInhibiting = value;
                        
                        if (attempt > 1)
                        {
                            logger?.LogInformation($"Successfully set master inhibit status to {value} on attempt {attempt}");
                        }
                        
                        return; // Success - exit
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        logger?.LogWarning($"Attempt {attempt}/{maxAttempts} to set master inhibit to {value} failed: {ex.Message}");
                        
                        if (attempt < maxAttempts)
                        {
                            // Wait before retry (except on last attempt)
                            Thread.Sleep(delayMs);
                        }
                    }
                }
                
                // All attempts failed - log critical error but don't update local state
                logger?.LogError($"Failed to set master inhibit status to {value} after {maxAttempts} attempts. Last error: {lastException?.Message}");
                
                // Don't update _isInhibiting if command failed
                // This keeps software state in sync with what we think hardware state is
                if (lastException != null)
                {
                    throw new DeviceCommunicationException($"Failed to set master inhibit status after {maxAttempts} attempts", lastException);
                }
                else
                {
                    throw new DeviceCommunicationException($"Failed to set master inhibit status after {maxAttempts} attempts");
                }
            }
        }

        BillIndex _allowedBills;
        /// <summary>
        /// Gets or sets the allowed indexes that this BillAcceptor is allowed to accept. Initially set to BillIndex.All
        /// </summary>
        public BillIndex AllowedBills
        {
            get { return _allowedBills; }
            set
            {
                if (_allowedBills == value) return;
                _allowedBills = value;
                base.CmdModifyInhibitStatus((byte)_allowedBills, (byte)((int)_allowedBills >> 8));
            }
        }

        /// <summary>
        /// *CURRENTLY UNUSED: interval hardcoded*
        /// Poll interval. By default value accepted from device while Init procedure. Can be changed, but next Init will reset value.
        /// If value = TimeSpan.Zero - device not recommends any intervals or use different hardware lines for polling.
        /// </summary>
        public TimeSpan PollInterval
        {
            get { return _pollInterval; }
            set
            {
                if (IsPolling)
                    throw new InvalidOperationException("Stop polling first");
                _pollInterval = value;
            }
        }

        /// <summary>
        ///  Starts poll events from device
        /// </summary>
        public void StartPoll()
        {
            if (_t != null)
                throw new InvalidOperationException("Stop polling first");

            lock (_timersSyncRoot)
            {
                if (!isCctalkDevInitialized)
                    throw new InvalidOperationException("Init first");

                // TODO: Use PollINterval instead of hardcoded value
                _t = new System.Timers.Timer(PollPeriod)
                {
                    AutoReset = false,
                };
                _t.Elapsed += PollTimerCallback;
                _t.Start();
            }
        }

        /// <summary>
        ///  Stops poll events from device
        /// </summary>
        public void EndPoll()
        {
            if (_t == null) return;
            lock (_timersSyncRoot)
            {
                _t.Elapsed -= PollTimerCallback;
                _t.Stop();
                _t.Dispose();
                _t = null;
            }
        }

        // Resume Polling after reconnect
        protected void ResumePollingAfterReconnect()
        {
            lock (_timersSyncRoot)
            {
                pausePollingDuringReconnect = false;
                reconnectionInProgress = false;
                _consecutiveErrorCount = 0; // Reset error count
                // TODO: Check which status to set here
                _lastKnownStatus = BillAcceptorDeviceStatus.Unknown; // Reset status
            }
        }

        /// <summary>
        ///  Returns the cached device status. Status is updated during polling.
        ///  If polling is not active, tries to get status via event buffer but only 
        ///  if enough time has passed since last check to avoid interfering with polling.
        /// </summary>
        public CctalkDeviceStatus GetStatus()
        {
            throw new NotImplementedException("Not implemeted status analysis from events.");
        }

        /// <summary>
        /// Sends a simple poll command to the device to check responsiveness.
        /// Returns true if the device responds successfully; otherwise, returns false.
        /// </summary>
        public bool SimplePoll()
        {
            try
            {
                base.CmdSimplePoll();
                return true;
            }
            catch (Exception ex)
            {
                logger?.LogDebug(ex, "Simple poll failed.");
                return false;
            }
        }

        /// <summary>
        /// Remembers current state of device`s event buffer as empty.
        /// All unread events in buffer will be discarded.
        /// </summary>
        public void ClearEventBuffer()
        {
            lock (_timersSyncRoot)
            {
                _isClearEventBufferRequested = true;
            }
        }

        /// <summary>
        /// Immediately executes poll process in blocking mode.
        /// All usual polling events will fire at calling thread.
        /// </summary>
        public void PollNow()
        {
            PollTimerCallback(this, EventArgs.Empty);
        }

        Boolean _isResetExpected = false;
        Boolean _isClearEventBufferRequested = false;

        void PollTimerCallback(object? sender, EventArgs e)
        {
            lock (_timersSyncRoot)
            {
                try
                {
                    // If reconnect in progress, skip polling
                    if (pausePollingDuringReconnect)
                    {
                        logger?.LogDebug("Polling paused during reconnection.");
                        return;
                    }

                    if(_lastKnownStatus == BillAcceptorDeviceStatus.Disconected)
                    {
                        // Set reconnect in progress flags
                        if(!reconnectionInProgress)
                        {
                            logger?.LogWarning("Device disconnected. Pausing polling and initiating reconnection.");
                            reconnectionInProgress = true;
                            pausePollingDuringReconnect = true;

                            // Notify derived classes about disconnection
                            DeviceDisconnectionDetected?.Invoke(this, EventArgs.Empty);
                        }
                        return;
                    }
                    // Use bill-specific event buffer command
                    var evBuf = base.CmdRequestBufferedBillEvents();
                    
                    // TODO: Analyze this case. Counter has maximum value 255 and rolls over to 0
                    // It does not indicate a reset, just a normal rollover -> should be analyzed
                    // Temporarily disabled portion of code
                    /*
                    var wasReset = evBuf.Counter < _lastEvent;

                    if (wasReset)
                    {
                        if (!_isResetExpected && _lastEvent != 0)
                        {
                            logger?.LogWarning("Device was reset unexpectedly");
                            var errorArgs = new BillAcceptorErrorEventArgs("Device reset detected");
                            RaiseInvokeErrorEvent(errorArgs);
                        }
                    }

                    if (_isClearEventBufferRequested)
                    {
                        _lastEvent = evBuf.Counter;
                        _isClearEventBufferRequested = false;
                    }
                    else
                    {
                        var newEventsCount = GetNewEventsCountHelper(_lastEvent, evBuf.Counter);
                        _lastEvent = evBuf.Counter;
                        RaiseEventsByBufferHelper(evBuf, newEventsCount);
                    }
                    */ // End of disabled portion

                    RaiseLastEvents(evBuf);

                    _consecutiveErrorCount = 0; // Reset error count on successful poll
                }
                catch (DeviceCommunicationException ex)
                {
                    _consecutiveErrorCount++;
                    logger?.LogError(ex, $"Device communication error (consecutive errors: {_consecutiveErrorCount})");

                    if (_lastKnownStatus != BillAcceptorDeviceStatus.Disconected)
                    {
                        // Update status to indicate communication problems
                        _lastKnownStatus = BillAcceptorDeviceStatus.Disconected;

                        var errorArgs = new BillAcceptorErrorEventArgs($"Communication error: {ex.Message}");
                        RaiseInvokeErrorEvent(errorArgs);
                    }

                    if (_consecutiveErrorCount >= MaxConsecutiveErrors && _t != null)
                    {
                        logger?.LogCritical("Maximum consecutive errors reached. Stopping polling.");
                        // TODO: Check if we should EndPoll
                        // EndPoll();
                    }
                }
                catch (Exception ex)
                {
                    _consecutiveErrorCount++;
                    logger?.LogError(ex, $"Unexpected error during polling (consecutive errors: {_consecutiveErrorCount})");

                    // Update status to indicate unexpected problems
                    _lastKnownStatus = BillAcceptorDeviceStatus.Error;

                    var errorArgs = new BillAcceptorErrorEventArgs($"Unexpected error: {ex.Message}");
                    RaiseInvokeErrorEvent(errorArgs);
                }
                finally
                {
                    if (_t != null && ReferenceEquals(sender, _t))
                        _t.Start();
                }
            }
        }
        
        private void RaiseLastEvents(DeviceEventBuffer buf)
        {
            var newEventsCount = GetNewEventsCountHelper(_lastEvent, buf.Counter);
            _lastEvent = buf.Counter;
            RaiseEventsByBufferHelper(buf, newEventsCount);
        }

        static Byte GetNewEventsCountHelper(Byte lastCounerVal, Byte newCounterVal)
        {
            if (newCounterVal == 0) return 0;

            var newEventsCount = (lastCounerVal <= newCounterVal)
                                    ? newCounterVal - lastCounerVal
                                    : (255 - lastCounerVal) + newCounterVal;

            return Convert.ToByte(newEventsCount);
        }

        void RaiseEventsByBufferHelper(DeviceEventBuffer buf, Byte countToShow)
        {
            if (countToShow == 0)
            {
                handleLastKnownStatusWithNoNewEvents();
                return;
            }

            var eventsLost = (countToShow > buf.Events.Length) ? (countToShow - buf.Events.Length) : 0;

            for (var i = 0; i < countToShow; i++)
            {
                var ev = buf.Events[i];

                // Handle bill events based on ccTalk bill acceptor specification
                if (ev.DeviceType == CcTalkDeviceType.BillAcceptor)
                {
                    var eventType = ev.GetBillEventType();

                    switch (eventType)
                    {
                        case BillEventType.Credit:

                            // Bill accepted or in escrow
                            if (_bills.ContainsKey(ev.BillType))
                            {
                                var bill = _bills[ev.BillType];
                                var billArgs = new BillAcceptorBillEventArgs(bill.Name, bill.Value, ev.BillType, ev.ActionCode);
                                RaiseInvokeBillEvent(billArgs);
                            }
                            else
                            {
                                // Unknown bill type
                                var billArgs = new BillAcceptorBillEventArgs($"Unknown Bill Type {ev.BillType}", 0, ev.BillType, ev.ActionCode);
                                RaiseInvokeBillEvent(billArgs);
                            }
                            break;

                        case BillEventType.PendingCredit:
                            logger?.LogInformation($"Bill pending credit: {ev.GetBillEventDescription()}");
                            logger?.LogWarning("Pending credit events are not fully supported and may indicate an issue.");
                            break;

                        case BillEventType.Reject:
                            logger?.LogInformation($"Bill rejected: {ev.GetBillEventDescription()}");
                            break;

                        case BillEventType.FraudAttempt:
                            logger?.LogWarning($"Fraud attempt detected: {ev.GetBillEventDescription()}");
                            break;

                        case BillEventType.FatalError:
                            // Error/problematic events
                            var errorArgs = new BillAcceptorErrorEventArgs(ev);
                            logger?.LogError($"FatalError detected: {ev.GetBillEventDescription()}");
                            logger?.LogError("Inhibiting further bill acceptance due to FatalError.");
                            _lastKnownStatus = BillAcceptorDeviceStatus.FatalError;
                            IsInhibiting = true; // Inhibit further accepting
                            RaiseInvokeErrorEvent(errorArgs);
                            break;

                        case BillEventType.Status:
                            // Status events - usually informational, log but don't always trigger events
                            logger?.LogInformation($"Bill acceptor status: {ev.GetBillEventDescription()}");

                            // Some status events might be worth reporting as errors
                            if (ev.ResultB == 11 || ev.ResultB == 14) // Stacker issues
                            {
                                var statusErrorArgs = new BillAcceptorErrorEventArgs(ev);
                                RaiseInvokeErrorEvent(statusErrorArgs);
                            }
                            break;

                        default:
                            // Unknown event types
                            logger?.LogWarning($"Unknown bill event: {ev}");
                            var unknownErrorArgs = new BillAcceptorErrorEventArgs($"Unknown bill event: {ev.GetBillEventDescription()}");
                            RaiseInvokeErrorEvent(unknownErrorArgs);
                            break;
                    }
                }
                else
                {
                    // Legacy coin acceptor event handling (shouldn't happen with bill acceptor)
                    logger?.LogWarning($"Received non-bill event: {ev}");
                }
            }

            if (eventsLost > 0)
            {
                var errorArgs = new BillAcceptorErrorEventArgs($"Lost {eventsLost} events due to buffer overflow");
                RaiseInvokeErrorEvent(errorArgs);
            }
        }

        void RaiseInvokeErrorEvent(BillAcceptorErrorEventArgs ea)
        {
            ErrorMessageAccepted?.Invoke(this, ea);
        }

        void RaiseInvokeBillEvent(BillAcceptorBillEventArgs ea)
        {
            _BillAccepted?.Invoke(this, ea);
        }

        void RaiseInvokeWarningEvent(BillAcceptorErrorEventArgs? ea = null, string? message = null)
        {
            if (WarningRaised != null)
            {
                if(ea != null)
                    WarningRaised(this, ea.ErrorMessage);
                else if(message != null)
                    WarningRaised(this, message);
            }
        }

        void handleLastKnownStatusWithNoNewEvents()
        {
            // No new events to process
            // This is normal when device is idle and functioning properly
            if (_lastKnownStatus == BillAcceptorDeviceStatus.FatalError)
            {
                // Once in FatalError state, remain there until reset or until new Events that idicate a change
                return;
            }
            // else if(_lastKnownStatus == BillAcceptorDeviceStatus.Disconected || _lastKnownStatus == BillAcceptorDeviceStatus.Reconnecting)
            // {
            //     return;
            // }
            else if (IsInhibiting)
            {
                _lastKnownStatus = BillAcceptorDeviceStatus.Inhibiting;
            }
            else if (IsPolling)
            {
                _lastKnownStatus = BillAcceptorDeviceStatus.ReadyForAccepting;
            }
        }

        private bool _disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                try
                {
                    UnInit();
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Error during UnInit in Dispose");
                }
                
                // evnt handlers cleanup
                try
                {
                    if (_BillAccepted != null)
                    {
                        foreach (var d in _BillAccepted.GetInvocationList())
                            _BillAccepted -= (EventHandler<BillAcceptorBillEventArgs>)d;
                    }
                    
                    if (ErrorMessageAccepted != null)
                    {
                        foreach (var d in ErrorMessageAccepted.GetInvocationList())
                            ErrorMessageAccepted -= (EventHandler<BillAcceptorErrorEventArgs>)d;
                    }
                    
                    if (WarningRaised != null)
                    {
                        foreach (var d in WarningRaised.GetInvocationList())
                            WarningRaised -= (EventHandler<string>)d;
                    }
                    
                    if (DeviceDisconnectionDetected != null)
                    {
                        foreach (var d in DeviceDisconnectionDetected.GetInvocationList())
                            DeviceDisconnectionDetected -= (EventHandler)d;
                    }
                    
                    logger?.LogDebug("Event handlers cleaned up successfully");
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Error cleaning up event handlers in Dispose");
                }
            }
            
            _disposed = true;
            
            base.Dispose(disposing);
        }

        /// <summary>
        /// builds correct config word from bills
        /// config word structure:
        /// {bill byte code}={bill value}={bill name};
        ///                 ^ splitter   ^ splitter  ^entry splitter
        /// </summary>
        /// <param name="bills">configuration to build the config word</param>
        /// <returns>config word itself</returns>
        public static string ConfigWord(Dictionary<byte, BillTypeInfo> bills)
        {
            var sb = new StringBuilder();
            foreach (var bill in bills)
            {
                sb.Append($"{bill.Key}={bill.Value.Value.ToString(CultureInfo.InvariantCulture)}={bill.Value.Name};");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Tries to parse config word and return bills info for the constructor
        /// {bill byte code}={bill value}={bill name};
        ///                 ^ splitter   ^ splitter  ^entry splitter
        /// </summary>
        /// <param name="word">config word</param>
        /// <param name="bills">out dictionary for parsed word, null if parsing fails</param>
        /// <returns>true for success, otherwise - false</returns>
        public static bool TryParseConfigWord(string word, out Dictionary<byte, BillTypeInfo>? bills)
        {
            bills = null;
            try
            {
                bills = new Dictionary<byte, BillTypeInfo>();
                var entries = word.Split(';');
                foreach (var entry in entries)
                {
                    if (string.IsNullOrWhiteSpace(entry)) continue;
                    var parts = entry.Split('=');
                    if (parts.Length != 3) continue;

                    var code = byte.Parse(parts[0]);
                    var value = decimal.Parse(parts[1], CultureInfo.InvariantCulture);
                    var name = parts[2];

                    bills[code] = new BillTypeInfo(name, value);
                }
                return true;
            }
            catch
            {
                bills = null;
                return false;
            }
        }
        
        protected void ModifyBillOperatingMode(bool stacker, bool escrow)
        {
            base.CmdModifyBillOperatingMode(stacker, escrow);
        }
    }
}