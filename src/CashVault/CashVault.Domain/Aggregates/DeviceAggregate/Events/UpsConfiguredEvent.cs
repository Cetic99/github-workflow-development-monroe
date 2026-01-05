using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class UpsConfiguredEvent : DeviceEvent
    {
        public UpsConfiguredEvent(UpsConfiguration upsConfiguration)
            : base("Ups configured")
        {
            UpsConfiguration = upsConfiguration;
        }
        public UpsConfiguration UpsConfiguration { get; private set; }
    }
}
