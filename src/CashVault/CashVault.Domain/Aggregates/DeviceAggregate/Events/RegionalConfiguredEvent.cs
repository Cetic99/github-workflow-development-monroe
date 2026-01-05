using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class RegionalConfiguredEvent : DeviceEvent
    {
        public RegionalConfiguredEvent(RegionalConfiguration regionalConfiguration)
            : base("Regional configured")
        {
            RegionalConfiguration = regionalConfiguration;
        }
        public RegionalConfiguration RegionalConfiguration { get; private set; }
    }
}
