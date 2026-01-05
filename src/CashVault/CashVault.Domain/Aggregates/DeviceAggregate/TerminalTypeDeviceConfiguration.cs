using System.Collections.Generic;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

public class TerminalTypeDeviceConfiguration
{
    private static readonly Dictionary<TerminalType, List<DeviceType>> Config =
        new()
        {
            {
                TerminalType.GamingATM,
                new List<DeviceType>
                {
                    DeviceType.BillAcceptor,
                    DeviceType.BillDispenser,
                    DeviceType.ThermalPrinter,
                    DeviceType.TITOPrinter,
                    DeviceType.UserCardReader
                }
            },
            {
                TerminalType.ParcelLocker,
                new List<DeviceType>
                {
                    DeviceType.BillAcceptor,
                    DeviceType.BillDispenser,
                    DeviceType.ThermalPrinter,
                    DeviceType.CoinAcceptor,
                    DeviceType.CoinDispenser,
                    DeviceType.CoinRecycler,
                    DeviceType.POSTerminal,
                    DeviceType.ParcelLocker
                }
            },
            {
                TerminalType.BankingAtm,
                new List<DeviceType>
                {
                    DeviceType.BillDispenser,
                    DeviceType.ThermalPrinter,
                    DeviceType.CoinAcceptor,
                    DeviceType.CoinDispenser,
                    DeviceType.CoinRecycler,
                    DeviceType.ICCardReader
                }
            },
            {
                TerminalType.Entertainment,
                new List<DeviceType>
                {
                    DeviceType.BillAcceptor,
                    DeviceType.ThermalPrinter,
                    DeviceType.CoinAcceptor,
                    DeviceType.POSTerminal
                }
            }
        };

    public List<DeviceType> GetSupportedDevicesByTerminalTypes(List<TerminalType> terminalTypes)
    {
        if (terminalTypes is null || terminalTypes.Count == 0)
        {
            return [];
        }

        var supportedDevices = new HashSet<DeviceType>();
        foreach (var terminalType in terminalTypes)
        {
            if (Config.TryGetValue(terminalType, out List<DeviceType>? value))
            {
                foreach (var deviceType in value)
                {
                    supportedDevices.Add(deviceType);
                }
            }
        }

        return new List<DeviceType>(supportedDevices);
    }

    public Dictionary<TerminalType, List<DeviceType>> GetConfig()
    {
        return Config;
    }
}
