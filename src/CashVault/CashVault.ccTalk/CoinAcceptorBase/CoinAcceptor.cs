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

namespace CashVault.ccTalk.CoinAcceptorBase
{

    /// <summary>
    /// Encapsulates various routines linked with coin acceptors. 
    /// Also invokes events to Wpf or Windows.Forms application/
    /// </summary>
    public abstract class CcTalkCoinAcceptorBase : BaseCctalkDevice, IDisposable
    {
        const Int32 PollPeriod = 500;
        const Int32 MaxConsecutiveErrors = 25;

        System.Timers.Timer _t;
        Byte _lastEvent;
        private int _consecutiveErrorCount;

        /// <summary>
        /// Fires when coin was accepted. Only when polling is on.
        /// </summary>
        protected event EventHandler<CoinAcceptorCoinEventArgs> _CoinAccepted;

        /// <summary>
        /// Fires when any error was detected during poll.
        /// </summary>
        protected event EventHandler<CoinAcceptorErrorEventArgs> ErrorMessageAccepted;

        TimeSpan _pollInterval;

        readonly Object _timersSyncRoot = new Object(); // for sync with timer threads only
        protected readonly Dictionary<Byte, String> _errors = new Dictionary<byte, string>();
        protected readonly Dictionary<Byte, CoinTypeInfo> _coins = new Dictionary<Byte, CoinTypeInfo>();

        private ILogger logger;
        public void ModifyAcceptedCoins(Dictionary<Byte, CoinTypeInfo> coins)
        {
            if (coins != null)
                foreach (var coin in coins)
                    _coins[coin.Key] = coin.Value;
        }

        public void Init(Boolean ignoreLastEvents = true)
        {
            //this.Connection.Open();

            DeviceCategory = base.CmdRequestEquipmentCategory();
            if (DeviceCategory != CctalkDeviceTypes.CoinAcceptor)
                throw new InvalidOperationException("Connected device is not a coin acceptor. " + DeviceCategory);

            //this.CmdReset();			
            base.CmdSetMasterInhibitStatus(IsInhibiting);

            //throw new InvalidOperationException("Msg: " + this.CmdGetMasterInhibitStatus().ToString()); - vraca false znaci da neInhibira.

            SerialNumber = base.CmdGetSerial();
            PollInterval = base.CmdRequestPollingPriority();
            Manufacturer = base.CmdRequestManufacturerId();
            ProductCode = base.CmdRequestProductCode();

            var evBuf = base.CmdReadEventBuffer();

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
                //this.Connection.Close();
                isCctalkDevInitialized = false;
            }

        }

        /// <summary>
        ///  true - port is open, ready for sending commands
        /// </summary>
        public Boolean isCctalkDevInitialized { get; private set; }

        /// <summary>
        ///  ccTalk address of device. 0 - broadcast.
        /// </summary>
        //public Byte Address { get { return this.Address; } }

        /// <summary>
        ///  Is polling is running now. Commands (as GetStatus) CAN be sent while polling.
        /// </summary>
        public Boolean IsPolling { get { return _t != null; } }

        protected String ProductCode { get; private set; }

        /// <summary>
        /// Serial number of device. Value accepted from device while Init.
        /// </summary>
        public Int32 SerialNumber { get; private set; }

        /// <summary>
        ///  Manufacter name of device. Value accepted from device while Init.
        /// </summary>
        public String Manufacturer { get; private set; }

        /// <summary>
        ///  Type of device. Value accepted from device while Init.
        /// </summary>
        public CctalkDeviceTypes DeviceCategory { get; private set; }

        bool _isInhibiting;

        /// <summary>
        ///  Indicates the state, when device is rejecting all coins.
        /// </summary>
        public Boolean IsInhibiting
        {
            get { return _isInhibiting; }
            set
            {
                base.CmdSetMasterInhibitStatus(value);
                _isInhibiting = value;
            }

        }

        CoinIndex _allowedCoins;
        /// <summary>
        /// Gets or sets the allowed indexes that this CoinAcceptor is allowed to accept. Initially set to CoinIndex.All
        /// </summary>
        public CoinIndex AllowedCoins
        {
            get { return _allowedCoins; }
            set
            {
                if (_allowedCoins == value) return;
                _allowedCoins = value;
                base.CmdModifyInhibitStatus((byte)_allowedCoins, (byte)((int)_allowedCoins >> 8));
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
                _t = new System.Timers.Timer(PollPeriod)
                {
                    AutoReset = false,
                };
                _t.Elapsed += TimerTick;
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
                _t.Elapsed -= TimerTick;
                _t.Stop();
                _t.Dispose();
                _t = null;
            }
        }

        /// <summary>
        ///  Polls the device immediatly. Returns true if device is ready. 
        ///  There is no need in calling this method when devise is polled.
        /// </summary>
        public CctalkDeviceStatus GetStatus()
        {
            if (!isCctalkDevInitialized) return CctalkDeviceStatus.OtherError;
            try
            {
                var status = base.CmdRequestStatus();
                return status;
            }
            //TODO: add specific exception handling for communication errors (e.g. DeviceCommunicationException)
            catch (Exception ex)
            {
                logger.LogError($"Failed to get status from device. Error: {ex.Message}");
                return CctalkDeviceStatus.OtherError;
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
            TimerTick(this, EventArgs.Empty);
        }

        Boolean _isResetExpected = false;
        Boolean _isClearEventBufferRequested = false;

        void TimerTick(object sender, EventArgs e)
        {
            lock (_timersSyncRoot)
            {
                //if(_t == null) return;
                try
                {
                    var buf = base.CmdReadEventBuffer();

                    // successful read - reset error counter
                    _consecutiveErrorCount = 0;

                    var wasReset = buf.Counter == 0;
                    if (wasReset)
                    {
                        if (!_isResetExpected && _lastEvent != 0)
                        {
                            RaiseInvokeErrorEvent(
                                new CoinAcceptorErrorEventArgs(
                                    CoinAcceptorErrors.UnspecifiedAlarmCode,
                                    "Unexpected reset"
                                    )
                                );
                        }
                    }

                    if (_isClearEventBufferRequested)
                    {
                        _lastEvent = buf.Counter;
                    }

                    _isResetExpected = false;

                    RaiseLastEvents(buf);
                }
                catch(DeviceCommunicationException ex)
                {
                    _consecutiveErrorCount++;
                    
                    logger.LogError($"Communication error during event polling: {ex.Message}. Consecutive errors: {_consecutiveErrorCount}");
                    
                    // Rise error event
                    RaiseInvokeErrorEvent(
                        new CoinAcceptorErrorEventArgs(
                            CoinAcceptorErrors.UnspecifiedAlarmCode,
                            $"Device communication error: {ex.Message}. Attempt {_consecutiveErrorCount} of {MaxConsecutiveErrors}"
                        )
                    );

                    // Check if max consecutive errors reached
                    if (_consecutiveErrorCount >= MaxConsecutiveErrors && _t != null)
                    {
                        logger.LogError("Max consecutive errors reached. Stopping poll and marking device as uninitialized.");
                        EndPoll();
                        isCctalkDevInitialized = false;
                        _consecutiveErrorCount = 0; // Reset for next attempt
                    }
                }
                catch (Exception ex)
                {
                    // Other errors
                    logger.LogError($"Error during event polling: {ex.Message}");
                    RaiseInvokeErrorEvent(
                        new CoinAcceptorErrorEventArgs(
                            CoinAcceptorErrors.UnspecifiedAlarmCode,
                            "Error during event polling: " + ex.Message
                            )
                        );
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

            var newEventsCount = lastCounerVal <= newCounterVal
                        ? newCounterVal - lastCounerVal
                        : (255 - lastCounerVal) + newCounterVal;

            return Convert.ToByte(newEventsCount);
        }

        void RaiseEventsByBufferHelper(DeviceEventBuffer buf, Byte countToShow)
        {
            if (countToShow == 0) return;

            for (var i = 0; i < Math.Min(countToShow, buf.Events.Length); i++)
            {
                var ev = buf.Events[i];
                if (ev.IsError)
                {
                    String errMsg;
                    var errCode = (CoinAcceptorErrors)ev.ErrorOrRouteCode;
                    _errors.TryGetValue(ev.ErrorOrRouteCode, out errMsg);
                    RaiseInvokeErrorEvent(new CoinAcceptorErrorEventArgs(errCode, errMsg));

                }
                else
                {
                    CoinTypeInfo coinInfo;
                    _coins.TryGetValue(ev.CoinCode, out coinInfo);
                    var evVal = coinInfo == null ? 0 : coinInfo.Value;
                    var evName = coinInfo == null ? null : coinInfo.Name;
                    RaiseInvokeCoinEvent(new CoinAcceptorCoinEventArgs(evName, evVal, ev.CoinCode, ev.ErrorOrRouteCode));
                }
            }

            var eventsLost = countToShow - buf.Events.Length;

            if (eventsLost > 0)
            {
                RaiseInvokeErrorEvent(new CoinAcceptorErrorEventArgs(CoinAcceptorErrors.UnspecifiedAlarmCode,
                                                                     "Events lost:" + eventsLost));
            }


        }

        void RaiseInvokeErrorEvent(CoinAcceptorErrorEventArgs ea)
        {
            if (ErrorMessageAccepted != null)
                ErrorMessageAccepted(this, ea);
        }

        void RaiseInvokeCoinEvent(CoinAcceptorCoinEventArgs ea)
        {
            if (_CoinAccepted != null)
                _CoinAccepted(this, ea);
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
                
                // event handlers cleanup
                try
                {
                    if (_CoinAccepted != null)
                    {
                        foreach (var d in _CoinAccepted.GetInvocationList())
                            _CoinAccepted -= (EventHandler<CoinAcceptorCoinEventArgs>)d;
                    }
                    
                    if (ErrorMessageAccepted != null)
                    {
                        foreach (var d in ErrorMessageAccepted.GetInvocationList())
                            ErrorMessageAccepted -= (EventHandler<CoinAcceptorErrorEventArgs>)d;
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
        /// bulids correct config word from coins
        /// config word structure:
        /// {coin byre code}={coin value}={coin name};
        ///                 ^ splitter   ^ splitter  ^entry splitter
        /// </summary>
        /// <param name="coins">configuration to build the config word</param>
        /// <returns>config word itself</returns>
        public static string ConfigWord(Dictionary<byte, CoinTypeInfo> coins)
        {
            var sb = new StringBuilder();
            foreach (var coin in coins)
            {
                sb.AppendFormat("{0}={1}={2};", coin.Key, coin.Value.Value, coin.Value.Name);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Trys to parse config word and return coins info for the constructor
        /// {coin byre code}={coin value}={coin name};
        ///                 ^ splitter   ^ splitter  ^entry splitter
        /// </summary>
        /// <param name="word">config word</param>
        /// <param name="coins">out dictionary for parsed word, null if parsing fails</param>
        /// <returns>true for success, otherwise - false</returns>
        public static bool TryParseConfigWord(string word, out Dictionary<byte, CoinTypeInfo> coins)
        {
            try
            {
                var coinWords = word.Split(';');
                coins = new Dictionary<byte, CoinTypeInfo>(coinWords.Length);
                foreach (var coinWord in coinWords)
                {
                    if (string.IsNullOrEmpty(coinWord))
                        continue;
                    var values = coinWord.Split('=');
                    var code = Byte.Parse(values[0]);
                    var value = decimal.Parse(values[1], NumberStyles.Currency);
                    var name = values.Length >= 3 ? values[2] : values[1];
                    coins[code] = new CoinTypeInfo(name, value);
                }
            }
            catch (Exception)
            {
                coins = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 1=2=2EUR; 2=1=1EUR; 3=0,5=50 cent; 4=0,2=20 cent; 5=0.1=10 cent; 6=0.05=5cent;		
        /// </summary>
        public static Dictionary<byte, CoinTypeInfo> DefaultConfig = new Dictionary<byte, CoinTypeInfo>
        {
            {1, new CoinTypeInfo("2 euro",  2M)},
            {2, new CoinTypeInfo("1 euro",  1M)},
            {3, new CoinTypeInfo("50 cent", 0.5M)},
            {4, new CoinTypeInfo("20 cent", 0.20M)},
            {5, new CoinTypeInfo("10 cent", 0.10M)},
            {6, new CoinTypeInfo("5 cent", 0.05M)},
        };

        public CcTalkCoinAcceptorBase(Port port, IServiceProvider serviceProvider) : base(port, serviceProvider)
        {
            logger = serviceProvider.GetRequiredService<ILogger<CcTalkCoinAcceptorBase>>();
        }
    }
}