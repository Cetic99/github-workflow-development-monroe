using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class NetworkConfiguredEvent : DeviceEvent
    {
        public NetworkConfiguredEvent(NetworkConfiguration networkConfiguration)
            : base("Network configured")
        {
            NetworkConfiguration = networkConfiguration;
        }
        public NetworkConfiguration NetworkConfiguration { get; private set; }
    }
}
