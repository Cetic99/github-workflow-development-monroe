using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class UserWidgetsUpdatedEvent : DeviceEvent
    {
        public UserWidgetsUpdatedEvent(UserWidgetsConfiguration userWidgetsConfiguration)
            : base($"User widgets configuration updated", CashVault.Domain.Aggregates.DeviceAggregate.DeviceType.General)
        {
            UserWidgetsConfiguration = userWidgetsConfiguration;
        }

        public UserWidgetsConfiguration UserWidgetsConfiguration { get; private set; }
    }
}
