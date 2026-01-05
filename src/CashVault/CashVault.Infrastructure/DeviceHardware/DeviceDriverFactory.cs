using System.IO.Ports;
using System.Text.Json;
using CashVault.BillAcceptorDriver.ID003.Config;
using CashVault.BillAcceptorDriver.NV10.Config;
using CashVault.BillDispenserDriver.JCM.F53.Config;
using CashVault.CabinetControlUnitDriver.Config;
using CashVault.CoinAcceptorDriver.RM5HD.Config;
using CashVault.DeviceDriver.Common;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Infrastructure.Extensions;
using CashVault.ParcelLockerDriver.Config;
using CashVault.TicketPrinterDriver.FutureLogic.Config;
using CashVault.UserCardReaderDriver.ACR1252U.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CashVault.Infrastructure.DeviceHardware;

public class DeviceDriverFactory : IDeviceDriverFactory
{
    private readonly JsonSerializerOptions _jsonSerializationOptions = new()
    {
        PropertyNamingPolicy = new LowerFirstLetterNamingPolicy()
    };
    private readonly JsonSerializerOptions _jsonDeserializationOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public MainConfiguration MainConfiguration { get; private set; }

    public void SetMainConfiguration(MainConfiguration mainConfig)
    {
        MainConfiguration = mainConfig ?? new MainConfiguration();
    }

    public JsonDocument? ConvertConfigurationToJson(IBasicHardwareDeviceConfiguration? deviceConfiguration)
    {
        if (deviceConfiguration == null)
        {
            return null;
        }
        var jsonString = JsonSerializer.Serialize(deviceConfiguration, deviceConfiguration.GetType(), _jsonSerializationOptions);
        var jsonDocument = JsonDocument.Parse(jsonString);
        return jsonDocument;
    }


    public IBasicHardwareDeviceConfiguration? CreateConfiguration(DeviceType deviceType, JsonDocument? configJson)
    {
        ArgumentNullException.ThrowIfNull(deviceType, nameof(deviceType));

        {
            var fullyQualifiedDriverName = MainConfiguration?.CabinetType?.FullyQualifiedDriverName;
            if (fullyQualifiedDriverName == "CashVault.CabinetControlUnitDriver")
            {
                if (configJson == null)
                {
                    return new CabinetControlUnitConfiguration();
                }
                var config = JsonSerializer.Deserialize<CabinetControlUnitConfiguration>(configJson.RootElement.GetRawText(), _jsonDeserializationOptions);
                return config;
            }
        }

        if (deviceType == DeviceType.BillDispenser)
        {
            var fullyQualifiedDriverName = MainConfiguration?.BillDispenserType?.FullyQualifiedDriverName;
            if (fullyQualifiedDriverName == "CashVault.BillDispenserDriver.JCM.F53")
            {
                if (configJson == null)
                {
                    return new BillDispenserJcm53Configuration();
                }
                var config = JsonSerializer.Deserialize<BillDispenserJcm53Configuration>(configJson.RootElement.GetRawText(), _jsonDeserializationOptions);
                return config;
            }
        }

        if (deviceType == DeviceType.BillAcceptor)
        {
            var fullyQualifiedDriverName = MainConfiguration?.BillAcceptorType?.FullyQualifiedDriverName;
            if (fullyQualifiedDriverName == "CashVault.BillAcceptorDriver.ID003")
            {
                if (configJson == null)
                {
                    return new BillAcceptorID003Configuration();
                }
                var config = JsonSerializer.Deserialize<BillAcceptorID003Configuration>(configJson.RootElement.GetRawText(), _jsonDeserializationOptions);
                return config;
            }
            else if (fullyQualifiedDriverName == "CashVault.BillAcceptorDriver.NV10")
            {
                if (configJson == null)
                {
                    return new BillAcceptorNV10Configuration();
                }
                var config = JsonSerializer.Deserialize<BillAcceptorNV10Configuration>(configJson.RootElement.GetRawText(), _jsonDeserializationOptions);
                return config;
            }
        }

        if (deviceType == DeviceType.UserCardReader)
        {
            var fullyQualifiedDriverName = MainConfiguration?.UserCardReaderType?.FullyQualifiedDriverName;
            if (fullyQualifiedDriverName == "CashVault.UserCardReaderDriver.ACR1252U")
            {
                if (configJson == null)
                {
                    return new UserCardReaderACR1252UConfiguration();
                }
                var config = JsonSerializer.Deserialize<UserCardReaderACR1252UConfiguration>(configJson.RootElement.GetRawText(), _jsonDeserializationOptions);
                return config;
            }
        }

        if (deviceType == DeviceType.TITOPrinter)
        {
            var fullyQualifiedDriverName = MainConfiguration?.TITOPrinterType?.FullyQualifiedDriverName;
            if (fullyQualifiedDriverName == "CashVault.TicketPrinterDriver.FutureLogic")
            {
                if (configJson == null)
                {
                    return new TITOPrinterFutureLogicConfiguration();
                }
                var config = JsonSerializer.Deserialize<TITOPrinterFutureLogicConfiguration>(configJson.RootElement.GetRawText(), _jsonDeserializationOptions);
                return config;
            }
        }

        if (deviceType == DeviceType.CoinAcceptor)
        {
            var fullyQualifiedDriverName = MainConfiguration?.CoinAcceptorType?.FullyQualifiedDriverName;
            if (fullyQualifiedDriverName == "CashVault.CoinAcceptorDriver.RM5HD")
            {
                if (configJson == null)
                {
                    return new CoinAcceptorRM5HDConfiguration();
                }
                var config = JsonSerializer.Deserialize<CoinAcceptorRM5HDConfiguration>(configJson.RootElement.GetRawText(), _jsonDeserializationOptions);
                return config;
            }
        }

        if (deviceType == DeviceType.ParcelLocker)
        {
            var fullyQualifiedDriverName = MainConfiguration?.ParcelLockerType?.FullyQualifiedDriverName;
            if (fullyQualifiedDriverName == "CashVault.ParcelLockerDriver")
            {
                if (configJson == null)
                {
                    return new ParcelLockerConfiguration();
                }
                var config = JsonSerializer.Deserialize<ParcelLockerConfiguration>(configJson.RootElement.GetRawText(), _jsonDeserializationOptions);
                return config;
            }
        }

        return null;
    }

    public IBasicHardwareDevice CreateDriver(string fullyQualifiedDriverName,
                                             Port port,
                                             IBasicHardwareDeviceConfiguration deviceConfiguration,
                                             IServiceProvider serviceProvider)
    {
        if (string.IsNullOrWhiteSpace(fullyQualifiedDriverName))
        {
            throw new ArgumentNullException(nameof(fullyQualifiedDriverName));
        }

        if (deviceConfiguration == null)
        {
            throw new ArgumentNullException(nameof(deviceConfiguration));
        }

        var localDevEnvOptions = serviceProvider.GetRequiredService<IOptions<LocalDevEnvOptions>>();
        var localDevEnvValue = localDevEnvOptions?.Value;

        if (fullyQualifiedDriverName == "CashVault.CabinetControlUnitDriver")
        {
            var config = (CabinetControlUnitConfiguration)deviceConfiguration;
            return new CabinetControlUnitDriver.CabinetControlUnitDriver(port, config, serviceProvider, localDevEnvValue);
        }
        else if (fullyQualifiedDriverName == "CashVault.BillDispenserDriver.JCM.F53")
        {
            var config = (BillDispenserJcm53Configuration)deviceConfiguration;
            return new BillDispenserDriver.JCM.F53.BillDispenserDriver(port, config, serviceProvider, localDevEnvValue);
        }
        else if (fullyQualifiedDriverName == "CashVault.BillAcceptorDriver.ID003")
        {
            BillAcceptorID003Configuration acceptorConfig = (BillAcceptorID003Configuration)deviceConfiguration;
            return new BillAcceptorDriver.ID003.BillAcceptorDriver(port, acceptorConfig, serviceProvider, localDevEnvValue);
        }
        else if (fullyQualifiedDriverName == "CashVault.BillAcceptorDriver.NV10")
        {
            BillAcceptorNV10Configuration acceptorConfig = (BillAcceptorNV10Configuration)deviceConfiguration;
            return new BillAcceptorDriver.NV10.BillAcceptorDriver(port, acceptorConfig, serviceProvider, localDevEnvValue);
        }
        else if (fullyQualifiedDriverName == "CashVault.TicketPrinterDriver.FutureLogic")
        {
            var config = (TITOPrinterFutureLogicConfiguration)deviceConfiguration;
            return new TicketPrinterDriver.FutureLogic.TicketPrinterDriver(port, config, serviceProvider, localDevEnvValue);
        }
        else if (fullyQualifiedDriverName == "CashVault.UserCardReaderDriver.ACR1252U")
        {
            try
            {
                var config = (UserCardReaderACR1252UConfiguration)deviceConfiguration;
                return new UserCardReaderDriver.ACR1252U.UserCardReaderDriver(serviceProvider, localDevEnvValue);
            }
            // TODO: This is temporary solution, this should be handled on UserCardReaderDriver
            catch { return null; }
        }
        else if (fullyQualifiedDriverName == "CashVault.CoinAcceptorDriver.RM5HD")
        {
            var config = (CoinAcceptorRM5HDConfiguration)deviceConfiguration;
            return new CoinAcceptorDriver.RM5HD.CoinAcceptorDriver(port, config, serviceProvider, localDevEnvValue);
        }
        else if (fullyQualifiedDriverName == "CashVault.ParcelLockerDriver")
        {
            var config = (ParcelLockerConfiguration)deviceConfiguration;
            return new ParcelLockerDriver.ParcelLockerDriver(port, config, serviceProvider, localDevEnvValue);
        }
        else
        {
            throw new ArgumentException("Invalid driver name", nameof(fullyQualifiedDriverName));
        }
    }

    public List<Port> GetAvailablePorts()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            string[] serialPortNames = SerialPort.GetPortNames();
            List<Port> result = new List<Port>();

            foreach (string portName in serialPortNames)
            {
                result.Add(new Port(PortType.Serial, portName, systemPortName: portName));
            }

            return result;
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            return GetUnixAvailableSerialPorts();
        }
        else
        {
            throw new PlatformNotSupportedException("Finding available serial ports failed.");
        }
    }

    public List<DeviceModel> GetSupportedDeviceModels()
    {
        return [
            new(DeviceType.Cabinet, "CashVault", "CabinetControlUnit", "CashVault.CabinetControlUnitDriver", "Cabinet control unit"),
            new(DeviceType.BillAcceptor, "JCM", "UBA-10-SS", "CashVault.BillAcceptorDriver.ID003", "Bill and ticket acceptor"),
            new(DeviceType.BillAcceptor, "ITL", "NV10", "CashVault.BillAcceptorDriver.NV10", "ccTalk Bill acceptor"),
            new(DeviceType.BillDispenser, "JCM", "F53", "CashVault.BillDispenserDriver.JCM.F53", "Bill dispenser"),
            new(DeviceType.TITOPrinter, "JCM/FutureLogic", "GEN2/GEN5", "CashVault.TicketPrinterDriver.FutureLogic", "Ticket printer"),
            new(DeviceType.UserCardReader, "ACS", "ACR1252U", "CashVault.UserCardReaderDriver.ACR1252U", "Card reader"),
            new(DeviceType.CoinAcceptor, "Comestero", "RM5 HD", "CashVault.CoinAcceptorDriver.RM5HD", "Coin acceptor"),
            new(DeviceType.ParcelLocker, "Monroe", "V1", "CashVault.ParcelLockerDriver", "Parcel locker"),
        ];
    }

    private List<Port> GetUnixAvailableSerialPorts()
    {
        List<Port> result = new List<Port>();
        var portNames = new HashSet<string>();

        // Add USB/ACM ports detected by C#
        string[] managedPorts = SerialPort.GetPortNames();
        foreach (string portName in managedPorts)
        {
            portNames.Add(portName);
        }

        // Find ttyS* ports that point to actual addresses (not serial8250)
        if (Directory.Exists("/sys/class/tty"))
        {
            try
            {
                var ttyDirs = Directory.GetDirectories("/sys/class/tty", "ttyS*");
                foreach (var ttyDir in ttyDirs)
                {
                    string ttyName = Path.GetFileName(ttyDir);
                    string deviceCheckPath = Path.Combine(ttyDir, "device", "tty", ttyName, "device");
                        
                    if (Directory.Exists(deviceCheckPath))
                    {
                        var deviceInfo = new DirectoryInfo(deviceCheckPath);
                        string deviceTarget = deviceInfo.LinkTarget ?? deviceInfo.FullName;
                            
                        // Ignore if it points to serial8250 (placeholders)
                        if (!deviceTarget.Contains("serial8250"))
                        {
                            string portPath = $"/dev/{ttyName}";
                            if (File.Exists(portPath))
                            {
                                portNames.Add(portPath);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors when reading /sys
            }
        }

        foreach (string portName in portNames.OrderBy(p => p))
        {
            string comName = MapLinuxPortToComName(portName);
            result.Add(new Port(PortType.Serial, comName, systemPortName: portName));
        }

        return result;
    }

    private string MapLinuxPortToComName(string linuxPortPath)
    {
        // Map ttyS* -> COM*
        if (linuxPortPath.StartsWith("/dev/ttyS"))
        {
            if (int.TryParse(linuxPortPath.Replace("/dev/ttyS", ""), out int portNumber))
            {
                return $"COM{portNumber + 1}"; // ttyS0 -> COM1, ttyS1 -> COM2, etc.
            }
        }
        // Map ttyUSB* -> USB-COM*
        if (linuxPortPath.StartsWith("/dev/ttyUSB"))
        {
            if (int.TryParse(linuxPortPath.Replace("/dev/ttyUSB", ""), out int portNumber))
            {
                return $"USB-COM{portNumber + 1}";
            }
        }
        // If we can't map it, return the original path
        return linuxPortPath;
    }
}
