using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;

public class BillAddedToAcceptorStackerEvent : TransactionEvent
{
    public BillAddedToAcceptorStackerEvent(BillTicketAcceptorStackerStatus oldAcceptorStatus, BillTicketAcceptorStackerStatus newAcceptorStatus)
    : base($"Bill added to acceptor stacker.")
    {
        OldAcceptorStackerStatus = oldAcceptorStatus;
        NewAcceptorStackerStatus = newAcceptorStatus;
    }

    public BillTicketAcceptorStackerStatus OldAcceptorStackerStatus { get; private set; }
    public BillTicketAcceptorStackerStatus NewAcceptorStackerStatus { get; private set; }
}
