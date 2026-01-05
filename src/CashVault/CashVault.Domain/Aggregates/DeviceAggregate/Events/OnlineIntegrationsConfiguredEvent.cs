using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class OnlineIntegrationsConfiguredEvent : DeviceEvent
    {
        public OnlineIntegrationsConfiguredEvent(OnlineIntegrationsConfiguration onlineIntegrationsConfiguration)
            : base("Online integrations configured")
        {
            OnlineIntegrationsConfiguration = onlineIntegrationsConfiguration;
        }
        public OnlineIntegrationsConfiguration OnlineIntegrationsConfiguration { get; private set; }
    }
}
