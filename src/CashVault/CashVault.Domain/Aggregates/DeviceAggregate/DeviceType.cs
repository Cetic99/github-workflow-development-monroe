using System.Text.Json.Serialization;
using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

[JsonConverter(typeof(EnumerationJsonConverter<DeviceType>))]
public class DeviceType : Enumeration
{
    public static DeviceType General = new(99, nameof(General).ToLowerInvariant());
    public static DeviceType BillDispenser = new DeviceType(1, nameof(BillDispenser).ToLowerInvariant());
    public static DeviceType BillAcceptor = new DeviceType(2, nameof(BillAcceptor).ToLowerInvariant());
    public static DeviceType UserCardReader = new DeviceType(3, nameof(UserCardReader).ToLowerInvariant());
    public static DeviceType TITOPrinter = new DeviceType(4, nameof(TITOPrinter).ToLowerInvariant());
    public static DeviceType Cabinet = new DeviceType(5, nameof(Cabinet).ToLowerInvariant());
    public static DeviceType ThermalPrinter = new DeviceType(6, nameof(ThermalPrinter).ToLowerInvariant());
    public static DeviceType CoinAcceptor = new DeviceType(7, nameof(CoinAcceptor).ToLowerInvariant());
    public static DeviceType CoinDispenser = new DeviceType(8, nameof(CoinDispenser).ToLowerInvariant());
    public static DeviceType CoinRecycler = new DeviceType(9, nameof(CoinRecycler).ToLowerInvariant());
    public static DeviceType POSTerminal = new DeviceType(10, nameof(POSTerminal).ToLowerInvariant());
    public static DeviceType ICCardReader = new DeviceType(11, nameof(ICCardReader).ToLowerInvariant());
    public static DeviceType ParcelLocker = new DeviceType(6, nameof(ParcelLocker).ToLowerInvariant());

    public DeviceType() { }

    public DeviceType(int id, string code)
        : base(id, code)
    {
    }
}