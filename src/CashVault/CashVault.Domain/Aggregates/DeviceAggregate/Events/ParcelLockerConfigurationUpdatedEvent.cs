using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class ParcelLockerConfigurationUpdatedEvent : DeviceEvent
{
    public ParcelLockerConfigurationUpdatedEvent(IParcelLockerConfiguration configuration)
        : base($"Parcel locker configuration updated", DeviceAggregate.DeviceType.General)
    {
        ParcelLockerConfiguration = configuration;
    }
    public IParcelLockerConfiguration ParcelLockerConfiguration { get; private set; }
}