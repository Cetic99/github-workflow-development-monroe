using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class ParcelLockerConfiguredEvent : DeviceEvent
{
    public IParcelLockerConfiguration ParcelLockerConfiguration { get; private set; }

    public ParcelLockerConfiguredEvent(IParcelLockerConfiguration configuration)
        : base("Parcel locker configured.", DeviceAggregate.DeviceType.ParcelLocker)
    {
        ParcelLockerConfiguration = configuration;
    }
}
