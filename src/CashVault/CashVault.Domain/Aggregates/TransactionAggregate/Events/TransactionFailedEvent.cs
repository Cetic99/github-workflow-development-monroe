using CashVault.Domain.Common.Events;
using CashVault.Domain.TransactionAggregate;

namespace CashVault.Domain.Aggregates.TransactionAggregate.Events;

public class TransactionFailedEvent : TransactionEvent
{
    public string Reason { get; set; }

    public TransactionFailedEvent(Transaction transaction, string reason)
    : base($"Transaction failed, reason: {reason}")
    {
        Reason = reason;
    }
}
