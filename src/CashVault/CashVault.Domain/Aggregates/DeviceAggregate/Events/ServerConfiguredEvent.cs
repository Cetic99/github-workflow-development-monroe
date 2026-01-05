using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class ServerConfiguredEvent : DeviceEvent
    {
        public ServerConfiguredEvent(ServerConfiguration serverConfiguration)
            : base("Server configured")
        {
            ServerConfiguration = serverConfiguration;
        }
        public ServerConfiguration ServerConfiguration { get; private set; }
    }
}
