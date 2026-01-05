using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class BillAcceptorConfiguredEvent : DeviceEvent
    {
        public BillAcceptorConfiguredEvent(IBillAcceptorConfiguration billAcceptorConfiguration)
            : base("Bill acceptor configured", DeviceAggregate.DeviceType.BillAcceptor)
        {
            BillAcceptorConfiguration = billAcceptorConfiguration;
        }
        public IBillAcceptorConfiguration BillAcceptorConfiguration { get; private set; }
    }
}
