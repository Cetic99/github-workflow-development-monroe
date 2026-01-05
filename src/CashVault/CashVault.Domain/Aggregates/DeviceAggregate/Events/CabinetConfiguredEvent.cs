using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class CabinetConfiguredEvent : DeviceEvent
{
    public ICabinetConfiguration CabinetConfiguration { get; private set; }

    public CabinetConfiguredEvent(ICabinetConfiguration cabinetConfiguration)
        : base("Cabinet configured", DeviceAggregate.DeviceType.Cabinet)
    {
        CabinetConfiguration = cabinetConfiguration;
    }
}
