using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events
{
    public class AcceptorEmptiedEvent : TransactionEvent
    {
        public AcceptorEmptiedEvent(BillTicketAcceptorStackerStatus oldAcceptorStatus, BillTicketAcceptorStackerStatus newAcceptorStatus)
            : base($"Bill acceptor emptied.")
        {
            OldAcceptorStackerStatus = oldAcceptorStatus;
            NewAcceptorStackerStatus = newAcceptorStatus;
        }

        public BillTicketAcceptorStackerStatus OldAcceptorStackerStatus { get; private set; }
        public BillTicketAcceptorStackerStatus NewAcceptorStackerStatus { get; private set; }
    }
}
