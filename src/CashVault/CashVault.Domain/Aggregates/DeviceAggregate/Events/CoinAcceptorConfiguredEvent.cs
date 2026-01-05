using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class CoinAcceptorConfiguredEvent : DeviceEvent
    {
        public CoinAcceptorConfiguredEvent(ICoinAcceptorConfiguration coinAcceptorConfiguration)
            : base("Coin acceptor configured", DeviceAggregate.DeviceType.CoinAcceptor)
        {
            CoinAcceptorConfiguration = coinAcceptorConfiguration;
        }
        public ICoinAcceptorConfiguration CoinAcceptorConfiguration { get; private set; }
    }
}
