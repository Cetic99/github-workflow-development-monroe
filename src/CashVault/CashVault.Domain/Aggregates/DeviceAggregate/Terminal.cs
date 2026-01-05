using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate
{
    public class Terminal : ITerminal
    {
        private readonly object _syncLock = new();
        private bool disposedValue;
        private List<BaseEvent> _domainEvents = [];
        private readonly IServiceProvider serviceProvider;
        private readonly IDeviceDriverFactory deviceDriverFactory;
        private SemaphoreSlim transactionSemaphore = new(1, 1); //Used to prevent multiple transactions at the same time
        private readonly SemaphoreSlim _serverActionsSemaphore = new(1, 1); //Used to prevent multiple server actions at the same time
        private readonly object _userLoginLock = new();
        private Timer? _userLoginTimer;
        private readonly TimeSpan _userLoginTimeout = TimeSpan.FromSeconds(20);

        [DllImport("libc")]
        public static extern uint getuid();

        #region Device properties

        public bool BillDispenserEnabled => Devices.Any(d => d.DeviceType == DeviceType.BillDispenser);
        public bool BillAcceptorEnabled => Devices.Any(d => d.DeviceType == DeviceType.BillAcceptor);
        public bool CardReaderEnabled => Devices.Any(d => d.DeviceType == DeviceType.UserCardReader);
        public bool TITOPrinterEnabled => Devices.Any(d => d.DeviceType == DeviceType.TITOPrinter);

        public IBillDispenser? BillDispenser { get; private set; }
        public ITITOPrinter? TITOPrinter { get; private set; }
        public IUserCardReader? UserCardReader { get; private set; }
        public IBillAcceptor? BillAcceptor { get; private set; }
        public ICoinAcceptor? CoinAcceptor { get; private set; }
        public ICabinet? Cabinet { get; private set; }
        public IParcelLocker? ParcelLocker { get; private set; }

        private readonly List<IBasicHardwareDevice> activeDrivers = new List<IBasicHardwareDevice>();

        public List<Port> AvailablePorts { get; private set; }

        public List<DeviceModel> AvailableDevices { get; private set; }

        public List<Device> Devices { get; private set; }

        public IBillAcceptorConfiguration BillAcceptorConfiguration { get; private set; }

        public IBillDispenserConfiguration BillDispenserConfiguration { get; private set; }

        public ITITOPrinterConfiguration TITOPrinterConfiguration { get; private set; }

        public ICabinetConfiguration CabinetConfiguration { get; private set; }

        public IUserCardReaderConfiguration UserCardReaderConfiguration { get; private set; }
        public IParcelLockerConfiguration ParcelLockerConfiguration { get; private set; }

        public ICoinAcceptorConfiguration CoinAcceptorConfiguration { get; private set; }
        #endregion

        #region Properties

        [JsonIgnore]
        public List<TerminalType> TerminalTypes { get; private set; }

        [JsonIgnore]
        public TerminalTypeDeviceConfiguration TerminalTypeDeviceConfiguration { get; private set; } = new TerminalTypeDeviceConfiguration();

        [JsonIgnore]

        private volatile TerminalOperatingMode _operatingMode;

        [JsonIgnore]
        private volatile bool transactionInProgress = false;

        public TerminalOperatingMode OperatingMode => _operatingMode;

        [JsonIgnore]
        public NetworkConfiguration NetworkConfiguration { get; private set; }

        [JsonIgnore]
        public UpsConfiguration UpsConfiguration { get; private set; }

        [JsonIgnore]
        public RegionalConfiguration RegionalConfiguration { get; private set; }

        [JsonIgnore]
        public UserWidgetsConfiguration UserWidgetsConfiguration { get; private set; }

        [JsonIgnore]
        public AvailableUserWidgetsConfiguration AvailableUserWidgetsConfiguration { get; private set; }

        [JsonIgnore]
        public OnlineIntegrationsConfiguration OnlineIntegrationsConfiguration { get; private set; }

        [JsonIgnore]
        public MainConfiguration MainConfiguration { get; private set; }

        [JsonIgnore]
        public ServerConfiguration ServerConfiguration { get; private set; }

        [JsonIgnore]
        public string? LocalTimeZone { get; private set; }

        [JsonIgnore]
        public int AmountPrecision { get; private set; } = 2;

        [JsonIgnore]
        public string TerminalStatus { get; private set; } = "OK";

        private bool _userLoginEnabled = false;

        [JsonIgnore]
        public bool UserLoginEnabled
        {
            get { lock (_userLoginLock) return _userLoginEnabled; }
            private set { lock (_userLoginLock) _userLoginEnabled = value; }
        }

        [JsonIgnore]
        public bool IsTransactionInProgress => transactionInProgress;

        #endregion

        #region Constructors

        public Terminal(IServiceProvider serviceProvider, IDeviceDriverFactory deviceDriverFactory)
        {
            TerminalTypes = [];
            TerminalTypeDeviceConfiguration = new TerminalTypeDeviceConfiguration();
            _operatingMode = TerminalOperatingMode.UnknownUser;

            this.serviceProvider =
                serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            this.deviceDriverFactory =
                deviceDriverFactory ?? throw new ArgumentNullException(nameof(deviceDriverFactory));

            AvailablePorts = GetAvailablePorts();
            AvailableDevices = GetAvailableDeviceModels();
            Devices = [];
        }

        #endregion

        #region Key actions

        public async Task StartAsync()
        {
            // TODO: Initialize all hardware devices
            if (Devices != null && Devices.Any())
                Devices.ForEach(device =>
                {
                    if (device.Port == null)
                    {
                        return;
                    }

                    string driverName = device.Model.FullyQualifiedDriverName;
                    Port port = device.Port;
                    IBasicHardwareDeviceConfiguration? deviceConfiguration = device.DeviceConfiguration;

                    IBasicHardwareDevice deviceDriver = deviceDriverFactory.CreateDriver(driverName, port, deviceConfiguration, serviceProvider);

                    if (deviceDriver is null)
                    {
                        return;
                    }

                    activeDrivers.Add(deviceDriver);
                    device.SetDeviceDriver(deviceDriver);

                    AssignDriverToProperties(device);
                });
        }

        public async Task ResetAsync()
        {
            await StopAsync();
            await StartAsync();

        }

        public async Task StopAsync()
        {
            foreach (var driver in activeDrivers)
            {
                driver.Dispose();
            }
            activeDrivers.Clear();
        }

        /// <summary>
        /// Assigns the driver to the corresponding property of the DeviceHub.
        /// </summary>
        /// <param name="device">Device that just has been started.</param>
        private void AssignDriverToProperties(Device device)
        {
            if (device.DeviceType == DeviceType.BillAcceptor)
            {
                BillAcceptor = device.DeviceDriver as IBillAcceptor;
            }
            if (device.DeviceType == DeviceType.BillDispenser)
            {
                BillDispenser = device.DeviceDriver as IBillDispenser;
            }
            if (device.DeviceType == DeviceType.TITOPrinter)
            {
                TITOPrinter = device.DeviceDriver as ITITOPrinter;
            }
            if (device.DeviceType == DeviceType.UserCardReader)
            {
                UserCardReader = device.DeviceDriver as IUserCardReader;
            }
            if (device.DeviceType == DeviceType.Cabinet)
            {
                Cabinet = device.DeviceDriver as ICabinet;
            }
            if (device.DeviceType == DeviceType.CoinAcceptor)
            {
                CoinAcceptor = device.DeviceDriver as ICoinAcceptor;
            }
            if (device.DeviceType == DeviceType.ParcelLocker)
            {
                ParcelLocker = device.DeviceDriver as IParcelLocker;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    StopAsync().Wait();

                    lock (_userLoginLock)
                    {
                        _userLoginTimer?.Dispose();
                        _userLoginTimer = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DeviceHub()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void SetOperatingMode(TerminalOperatingMode operatingMode)
        {
            // TODO: dispatch event
            _operatingMode = operatingMode;

            if (Devices.Any())
                foreach (var device in Devices)
                    device?.DeviceDriver?.SetOperatingMode(operatingMode);
        }

        public IBasicHardwareDeviceConfiguration? GetDeviceConfiguration(DeviceType deviceType)
        {
            var device = Devices.FirstOrDefault(d => d.DeviceType == deviceType);
            if (device != null)
            {
                return device.DeviceConfiguration;
            }
            return null;
        }

        public IBasicHardwareDeviceConfiguration CreateAndValidateDeviceConfiguration(DeviceType deviceType, JsonDocument configurationJson)
        {
            var billDispenser = (Devices?.FirstOrDefault(d => d.DeviceType == DeviceType.BillDispenser))
                ?? throw new NotSupportedException($"Device of type {deviceType} not found");

            var fullyQualifiedDriverName = billDispenser.Model.FullyQualifiedDriverName;
            var config = deviceDriverFactory.CreateConfiguration(deviceType, configurationJson);
            config.Validate();

            return config;
        }
        #endregion

        #region Device actions

        public async Task<bool> ResetDeviceAsync(DeviceType deviceType)
        {
            IBasicHardwareDevice? deviceDriver = Devices.FirstOrDefault(d => d.DeviceType == deviceType)?.DeviceDriver;

            if (deviceDriver != null)
            {
                return await deviceDriver.ResetAsync();
            }

            return false;
        }

        public async Task<bool> EnableDeviceAsync(DeviceType deviceType)
        {
            IBasicHardwareDevice? deviceDriver = Devices.FirstOrDefault(d => d.DeviceType == deviceType)?.DeviceDriver;

            if (deviceDriver != null)
            {
                return await deviceDriver.EnableAsync();
            }

            return false;
        }

        public async Task<bool> DisableDeviceAsync(DeviceType deviceType)
        {
            IBasicHardwareDevice? deviceDriver = Devices.FirstOrDefault(d => d.DeviceType == deviceType)?.DeviceDriver;

            if (deviceDriver != null)
            {
                return await deviceDriver.DisableAsync();
            }

            return false;
        }

        public List<IBasicHardwareDevice> GetDevicesAsync()
        {
            var targetDevices = new List<IBasicHardwareDevice>();

            Devices.ToList().ForEach(device =>
            {
                if (device.DeviceDriver != null)
                {
                    targetDevices.Add(device.DeviceDriver);
                }
            });

            return targetDevices;
        }

        public void RemoveDevice(Device device)
        {
            device?.DeviceDriver?.Dispose();
            Devices.Remove(device);
        }

        public IBasicHardwareDevice? GetDeviceByType(DeviceType deviceType)
        {
            return Devices.FirstOrDefault(d => d.DeviceType == deviceType)?.DeviceDriver;
        }

        public void AddDevice(Device device)
        {
            // TODO: temporary disabled during development
            // if (device.Port != null && AvailablePorts.Contains(device.Port) == false)
            // {
            //     throw new InvalidOperationException($"Port {device.Port} is not available on this terminal");
            // }

            if (AvailableDevices.Contains(device.Model) == false)
            {
                throw new InvalidOperationException($"Device {device.Model} is not available on this terminal");
            }

            Devices.Add(device);
            // TODO: check this
        }

        public void AddDeviceError(DeviceFailEvent deviceFailEvent)
        {
            ArgumentNullException.ThrowIfNull(deviceFailEvent, nameof(deviceFailEvent));
            AddDomainEvent(deviceFailEvent);
        }

        public List<DeviceModel> GetAvailableDeviceModels()
        {
            return deviceDriverFactory.GetSupportedDeviceModels();
        }

        public void AddWarningRaised(DeviceWarningRaisedEvent warningRaisedEvent)
        {
            ArgumentNullException.ThrowIfNull(warningRaisedEvent, nameof(warningRaisedEvent));
            AddDomainEvent(warningRaisedEvent);
        }

        public void SetTerminalStatus(string status)
        {
            TerminalStatus = status;
        }

        [CanBeCalledFromRemoteServer]
        public void PrintMessage(string deviceId)
        {
            Console.WriteLine($"Message from method: {deviceId}");
        }
        #endregion

        #region Config actions

        public void SetCabinetConfiguration(ICabinetConfiguration cabinetConfiguration)
        {
            CabinetConfiguration = cabinetConfiguration;

            var cabinet = Devices?.FirstOrDefault(d => d.DeviceType == DeviceType.Cabinet);
            if (cabinet != null)
            {
                cabinet.SetDeviceConfiguration(cabinetConfiguration);
                cabinet?.DeviceDriver?.ResetAsync(cabinetConfiguration);
            }

            AddDomainEvent(new CabinetConfiguredEvent(cabinetConfiguration));
        }

        public void SetUserCardReaderConfiguration(IUserCardReaderConfiguration cardReaderConfiguration)
        {
            UserCardReaderConfiguration = cardReaderConfiguration;
            var cardReader = Devices?.FirstOrDefault(d => d.DeviceType == DeviceType.UserCardReader);
            if (cardReader != null)
            {
                cardReader.SetDeviceConfiguration(cardReaderConfiguration);
                cardReader?.DeviceDriver?.ResetAsync(cardReaderConfiguration);
            }

            AddDomainEvent(new UserCardReaderConfiguredEvent(cardReaderConfiguration));
        }

        public void SetBillDispenserConfiguration(IBillDispenserConfiguration billDispenserConfiguration)
        {
            BillDispenserConfiguration = billDispenserConfiguration;

            var billDispenser = Devices?.FirstOrDefault(d => d.DeviceType == DeviceType.BillDispenser);
            if (billDispenser != null)
            {
                billDispenser.SetDeviceConfiguration(billDispenserConfiguration);
                billDispenser?.DeviceDriver?.ResetAsync(billDispenserConfiguration);
            }

            AddDomainEvent(new BillDispenserConfiguredEvent(billDispenserConfiguration));
        }

        public void SetBillAcceptorConfiguration(IBillAcceptorConfiguration billAcceptorConfiguration)
        {
            BillAcceptorConfiguration = billAcceptorConfiguration;

            var billAcceptor = Devices?.FirstOrDefault(d => d.DeviceType == DeviceType.BillAcceptor);
            if (billAcceptor != null)
            {
                billAcceptor.SetDeviceConfiguration(billAcceptorConfiguration);
                billAcceptor?.DeviceDriver?.ResetAsync(billAcceptorConfiguration);
            }

            AddDomainEvent(new BillAcceptorConfiguredEvent(billAcceptorConfiguration));
        }

        public void SetTITOPrinterConfiguration(ITITOPrinterConfiguration titoPrinterConfiguration)
        {
            TITOPrinterConfiguration = titoPrinterConfiguration;

            var titoPrinter = Devices?.FirstOrDefault(d => d.DeviceType == DeviceType.TITOPrinter);
            if (titoPrinter != null)
            {
                titoPrinter.SetDeviceConfiguration(titoPrinterConfiguration);
                titoPrinter?.DeviceDriver?.ResetAsync(titoPrinterConfiguration);
            }

            AddDomainEvent(new TITOPrinterConfiguredEvent(titoPrinterConfiguration));
        }

        public void SetCoinAcceptorConfiguration(ICoinAcceptorConfiguration coinAcceptorConfiguration)
        {
            CoinAcceptorConfiguration = coinAcceptorConfiguration;
            var coinAcceptor = Devices?.FirstOrDefault(d => d.DeviceType == DeviceType.CoinAcceptor);
            if (coinAcceptor != null)
            {
                coinAcceptor.SetDeviceConfiguration(coinAcceptorConfiguration);
                coinAcceptor?.DeviceDriver?.ResetAsync(coinAcceptorConfiguration);
            }
            AddDomainEvent(new CoinAcceptorConfiguredEvent(coinAcceptorConfiguration));
        }

        public void SetParcelLockerConfiguration(IParcelLockerConfiguration parcelLockerConfig)
        {
            ParcelLockerConfiguration = parcelLockerConfig;

            var parcelLocker = Devices?.FirstOrDefault(d => d.DeviceType == DeviceType.ParcelLocker);
            if (parcelLocker != null)
            {
                parcelLocker.SetDeviceConfiguration(parcelLockerConfig);
                parcelLocker?.DeviceDriver?.ResetAsync(parcelLockerConfig);
            }

            AddDomainEvent(new ParcelLockerConfiguredEvent(parcelLockerConfig));
        }

        /// <summary>
        /// This method sets the network configuration of the terminal.
        /// Application must have administrative rights to execute this method!!!
        /// </summary>
        /// <param name="networkConfiguration"></param>
        /// <returns></returns>
        public string SetNetworkConfiguration(NetworkConfiguration networkConfiguration)
        {
            NetworkConfiguration = networkConfiguration;
            string scriptName = "Set-NetworkAdapter.ps1";

            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", scriptName);

            string result = "";

            if (networkConfiguration.NetworkAdaptersConfig != null && networkConfiguration.NetworkAdaptersConfig.Count > 0)
            {
                foreach (var adapter in networkConfiguration.NetworkAdaptersConfig)
                {
                    result += UpdateNetworkAdapter(
                        scriptPath,
                        adapter.Name,
                        adapter.IpAddress,
                        adapter.NetworkMask,
                        adapter.Gateway,
                        adapter.PreferredDns,
                        adapter.AlternateDns,
                        adapter.IsDhcpEnabled,
                        adapter.IsDnsEnabled);
                }
            }

            AddDomainEvent(new NetworkConfiguredEvent(networkConfiguration));
            return result;
        }

        public void SetUpsConfiguration(UpsConfiguration upsConfiguration)
        {
            UpsConfiguration = upsConfiguration;
            AddDomainEvent(new UpsConfiguredEvent(upsConfiguration));
        }

        public void SetRegionalConfiguration(RegionalConfiguration regionalConfiguration)
        {
            RegionalConfiguration = regionalConfiguration;
            AddDomainEvent(new RegionalConfiguredEvent(regionalConfiguration));
        }

        public void UpdateUserWidgetsConfiguration(UserWidgetsConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            UserWidgetsConfiguration = configuration;
        }

        [CanBeCalledFromRemoteServer]
        public void SetUserWidgetsConfiguration(UserWidgetsConfiguration configuration)
        {
            UpdateUserWidgetsConfiguration(configuration);
            AddDomainEvent(new UserWidgetsUpdatedEvent(configuration));
        }

        [CanBeCalledFromRemoteServer]
        public void SetAvailableUserWidgetsConfiguration(AvailableUserWidgetsConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            AvailableUserWidgetsConfiguration = configuration;
            AddDomainEvent(new AvailableUserWidgetsConfigurationUpdatedEvent(configuration));
        }

        public void SetOnlineIntegrationsConfiguration(OnlineIntegrationsConfiguration onlineIntegrationsConfiguration)
        {
            OnlineIntegrationsConfiguration = onlineIntegrationsConfiguration;
            AddDomainEvent(new OnlineIntegrationsConfiguredEvent(onlineIntegrationsConfiguration));
        }

        public void SetServerConfiguration(ServerConfiguration serverConfiguration)
        {
            ServerConfiguration = serverConfiguration;
            AddDomainEvent(new ServerConfiguredEvent(serverConfiguration));
        }

        public void SetMainConfiguration(MainConfiguration mainConfiguration)
        {
            ArgumentNullException.ThrowIfNull(mainConfiguration, nameof(mainConfiguration));

            Devices.ForEach(d => { d?.DeviceDriver?.Dispose(); });
            Devices.Clear();

            if (mainConfiguration.IsCabinetMainConfigUpdated())
            {
                CabinetConfiguration ??= (ICabinetConfiguration)deviceDriverFactory.CreateConfiguration(DeviceType.Cabinet, null);
                var cabinetDevice = new Device(mainConfiguration.CabinetType, DeviceType.Cabinet);
                cabinetDevice.SetDeviceConfiguration(CabinetConfiguration);
                var cabinetPort = mainConfiguration.CabinetInterface;
                cabinetDevice.SetPort(cabinetPort);

                Devices.Add(cabinetDevice);
            }

            if (mainConfiguration.IsUserCardReaderMainConfigUpdated())
            {
                UserCardReaderConfiguration ??= (IUserCardReaderConfiguration)deviceDriverFactory.CreateConfiguration(DeviceType.UserCardReader, null);
                var userCardReaderDevice = new Device(mainConfiguration.UserCardReaderType, DeviceType.UserCardReader);
                userCardReaderDevice.SetDeviceConfiguration(UserCardReaderConfiguration);
                var userCardReaderPort = mainConfiguration.UserCardReaderInterface;
                userCardReaderDevice.SetPort(userCardReaderPort);

                Devices.Add(userCardReaderDevice);
            }

            if (mainConfiguration.IsBillDispenserMainConfigUpdated())
            {
                BillDispenserConfiguration ??= (IBillDispenserConfiguration)deviceDriverFactory.CreateConfiguration(DeviceType.BillDispenser, null);
                var dispenserDevice = new Device(mainConfiguration.BillDispenserType, DeviceType.BillDispenser);
                dispenserDevice.SetDeviceConfiguration(BillDispenserConfiguration);
                var dispenserPort = mainConfiguration.BillDispenserInterface;
                dispenserDevice.SetPort(dispenserPort);

                Devices.Add(dispenserDevice);
            }

            if (mainConfiguration.IsTITOPrinterMainConfigUpdated())
            {
                TITOPrinterConfiguration ??= (ITITOPrinterConfiguration)deviceDriverFactory.CreateConfiguration(DeviceType.TITOPrinter, null);
                var titoPrinterDevice = new Device(mainConfiguration.TITOPrinterType, DeviceType.TITOPrinter);
                titoPrinterDevice.SetDeviceConfiguration(TITOPrinterConfiguration);
                var ticketPrinterPort = mainConfiguration.TITOPrinterInterface;
                titoPrinterDevice.SetPort(ticketPrinterPort);

                Devices.Add(titoPrinterDevice);
            }

            if (mainConfiguration.IsBillAcceptorMainConfigUpdated())
            {
                BillAcceptorConfiguration ??= (IBillAcceptorConfiguration)deviceDriverFactory.CreateConfiguration(DeviceType.BillAcceptor, null);
                var acceptorDevice = new Device(mainConfiguration.BillAcceptorType, DeviceType.BillAcceptor);
                acceptorDevice.SetDeviceConfiguration(BillAcceptorConfiguration);
                var acceptorPort = mainConfiguration.BillAcceptorInterface;
                acceptorDevice.SetPort(acceptorPort);

                Devices.Add(acceptorDevice);
            }

            if (mainConfiguration.IsCoinAcceptorMainConfigUpdated())
            {
                CoinAcceptorConfiguration ??= (ICoinAcceptorConfiguration)deviceDriverFactory.CreateConfiguration(DeviceType.CoinAcceptor, null);
                var coinAcceptorDevice = new Device(mainConfiguration.CoinAcceptorType, DeviceType.CoinAcceptor);
                coinAcceptorDevice.SetDeviceConfiguration(CoinAcceptorConfiguration);
                var coinAcceptorPort = mainConfiguration.CoinAcceptorInterface;
                coinAcceptorDevice.SetPort(coinAcceptorPort);

                Devices.Add(coinAcceptorDevice);
            }

            if (mainConfiguration.IsParcelLockerMainConfigUpdated())
            {
                ParcelLockerConfiguration ??= (IParcelLockerConfiguration)deviceDriverFactory.CreateConfiguration(DeviceType.ParcelLocker, null);
                var parcelLockerDevice = new Device(mainConfiguration.ParcelLockerType, DeviceType.ParcelLocker);
                parcelLockerDevice.SetDeviceConfiguration(ParcelLockerConfiguration);
                var parcelLockerPort = mainConfiguration.ParcelLockerInterface;
                parcelLockerDevice.SetPort(parcelLockerPort);

                Devices.Add(parcelLockerDevice);
            }

            MainConfiguration = mainConfiguration;
            TerminalTypes = mainConfiguration.TerminalTypes;
        }

        #endregion

        #region Other actions
        public List<Port> GetAvailablePorts()
        {
            return deviceDriverFactory?.GetAvailablePorts() ?? new List<Port>();
        }

        public bool IsPortAvailable(string portName)
        {
            if (deviceDriverFactory == null) return false;

            return deviceDriverFactory.GetAvailablePorts().Any(x => x.Name == portName);
        }

        public void UpdateLocalTimeZone(string? timeZone)
        {
            LocalTimeZone = timeZone;
        }

        public void UpdateAmountPrecision(int amountPrecision)
        {
            AmountPrecision = amountPrecision;
        }

        private static string UpdateNetworkAdapter(string scriptPath,
                                           string adapterName,
                                           string? ipAddress,
                                           string? netMask,
                                           string? gateway,
                                           string? preferredDns,
                                           string? alternateDns,
                                           bool isDhcpEnabled,
                                           bool isDnsEnabled)
        {
            StringBuilder sb = new();

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string shell;

            if (isWindows)
            {
                shell = "powershell.exe";
            }
            else
            {
                shell = "pwsh"; //TODO: test script on linux
            }

            sb.Append($" -ExecutionPolicy Unrestricted -File \"{scriptPath}\"");
            sb.Append($" -AdapterName \"{adapterName}\" -IpAddress \"{ipAddress}\" -NetMask \"{netMask}\" -PrefixLength \"{NetworkConfiguration.MaskToPrefixLength(netMask)}\"");
            sb.Append($" -IsDhcpEnabled \"{isDhcpEnabled}\" -IsDnsEnabled \"{isDnsEnabled}\" -Gateway \"{gateway}\"");

            if (!string.IsNullOrEmpty(preferredDns))
                sb.Append($" -PreferredDns \"{preferredDns}\"");
            if (!string.IsNullOrEmpty(alternateDns))
                sb.Append($" -AlternateDns \"{alternateDns}\"");

            ProcessStartInfo psi = new()
            {
                FileName = shell,
                Arguments = sb.ToString(),
                Verb = isWindows ? "runas" : "",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                using Process process = new() { StartInfo = psi };
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(error))
                {
                    return "Error: " + error;
                }

                return output;
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.Message;
            }
        }


        public void SetUserLoginEnabled(bool enabled)
        {
            lock (_userLoginLock)
            {
                UserLoginEnabled = enabled;

                if (enabled)
                {
                    if (_userLoginTimer == null)
                    {
                        _userLoginTimer = new System.Threading.Timer(UserLoginTimeoutCallback, state: null, _userLoginTimeout, Timeout.InfiniteTimeSpan);
                    }
                    else
                    {
                        _userLoginTimer.Change(_userLoginTimeout, Timeout.InfiniteTimeSpan);
                    }
                }
                else
                {
                    if (_userLoginTimer != null)
                    {
                        _userLoginTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        _userLoginTimer.Dispose();
                        _userLoginTimer = null;
                    }
                }
            }
        }

        private void UserLoginTimeoutCallback(object? state)
        {
            lock (_userLoginLock)
            {
                UserLoginEnabled = false;

                if (_userLoginTimer != null)
                {
                    _userLoginTimer.Dispose();
                    _userLoginTimer = null;
                }
            }
        }

        #endregion

        #region Server actions

        private readonly List<TerminalServerAction> _pendingServerActions = [];
        public List<TerminalServerAction> CompletedServerActions => _pendingServerActions?.Where(x => x.IsCompleted).ToList() ?? [];

        public async Task AddServerActionAsync(TerminalServerAction action)
        {
            await _serverActionsSemaphore.WaitAsync();
            try
            {
                _pendingServerActions.Add(action);
                if (IsTransactionInProgress)
                {
                    MarkServerActionCompletedAsync(action.Uuid, success: false, "Transaction in progress");
                }
                else
                {
                    action.Method.Invoke(this, action.Parameters);
                    MarkServerActionCompletedAsync(action.Uuid, success: true, responseMessage: "Success!");
                }
            }
            catch (Exception ex)
            {
                MarkServerActionCompletedAsync(action.Uuid, success: false, ex?.Message ?? ex?.InnerException?.Message);
            }
            finally
            {
                _serverActionsSemaphore.Release();
            }
        }

        public async Task RemoveServerActionAsync(Guid uuid)
        {
            await _serverActionsSemaphore.WaitAsync();
            try
            {
                _pendingServerActions.RemoveAll(a => a.Uuid == uuid);
            }
            finally
            {
                _serverActionsSemaphore.Release();
            }
        }

        private void MarkServerActionCompletedAsync(Guid uuid, bool success, string? responseMessage = null)
        {
            var action = _pendingServerActions.FirstOrDefault(a => a.Uuid == uuid);
            if (action != null)
            {
                action.IsCompleted = true;
                action.IsSuccess = success;
                action.ResponseMessage = responseMessage;
            }
        }

        #endregion

        #region Domain events

        [JsonIgnore]
        public List<BaseEvent> DomainEvents
        {
            get
            {
                lock (_syncLock)
                {
                    return new List<BaseEvent>(_domainEvents);
                }
            }
        }

        public void AddDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents ??= [];

            if (domainEvent == null)
            {
                throw new ArgumentNullException(nameof(domainEvent), "Domain event cannot be null.");
            }

            lock (_syncLock)
            {
                _domainEvents.Add(domainEvent);
            }
        }

        public void ClearDomainEvents()
        {
            lock (_syncLock)
            {
                _domainEvents.Clear();
            }
        }

        public async Task<bool> StartTransaction()
        {
            await transactionSemaphore.WaitAsync();

            try
            {
                if (transactionInProgress)
                {
                    return false;
                }

                transactionInProgress = true;
                return true;
            }
            finally
            {
                transactionSemaphore.Release();
            }
        }

        public async Task EndTransaction()
        {
            await transactionSemaphore.WaitAsync();

            try
            {
                transactionInProgress = false;
            }
            finally
            {
                transactionSemaphore.Release();
            }
        }

        public static bool AppHasAdminPrivilages()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                bool isAdmin;
                using (var identity = WindowsIdentity.GetCurrent())
                {
                    var principal = new WindowsPrincipal(identity);
                    isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
                }

                return isAdmin;
            }
            else
            {
                return getuid() == 0;
            }
        }
        #endregion
    }
}