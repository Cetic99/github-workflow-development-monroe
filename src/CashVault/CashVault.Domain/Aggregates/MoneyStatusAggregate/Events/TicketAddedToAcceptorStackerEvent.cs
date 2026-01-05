using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;

public class TicketAddedToAcceptorStackerEvent : TransactionEvent
{
    public TicketAddedToAcceptorStackerEvent(BillTicketAcceptorStackerStatus oldAcceptorStatus, BillTicketAcceptorStackerStatus newAcceptorStatus)
        : base($"Ticket added to acceptor stacker.")
    {
        OldAcceptorStackerStatus = oldAcceptorStatus;
        NewAcceptorStackerStatus = newAcceptorStatus;
    }

    public BillTicketAcceptorStackerStatus OldAcceptorStackerStatus { get; private set; }
    public BillTicketAcceptorStackerStatus NewAcceptorStackerStatus { get; private set; }
}
