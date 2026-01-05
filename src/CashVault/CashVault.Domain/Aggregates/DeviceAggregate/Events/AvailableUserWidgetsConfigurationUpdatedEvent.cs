using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class AvailableUserWidgetsConfigurationUpdatedEvent : DeviceEvent
{
    public AvailableUserWidgetsConfigurationUpdatedEvent(AvailableUserWidgetsConfiguration configuration)
        : base($"Available user widgets configuration updated", CashVault.Domain.Aggregates.DeviceAggregate.DeviceType.General)
    {
        AvailableUserWidgets = configuration;
    }
    public AvailableUserWidgetsConfiguration AvailableUserWidgets { get; private set; }
}
