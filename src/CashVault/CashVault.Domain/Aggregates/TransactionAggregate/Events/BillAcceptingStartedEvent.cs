using CashVault.Domain.Common.Events;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.TransactionAggregate.Events;

public class BillAcceptingStartedEvent : TransactionEvent
{
    public BillAcceptingStartedEvent()
        : base($"Bill/Ticket accepting started")
    {
       
    }
}
