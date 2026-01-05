using CashVault.Domain.Common.Events;
using CashVault.Domain.TransactionAggregate;

namespace CashVault.Domain.Aggregates.TransactionAggregate.Events;

public class TransactionRequestedEvent : TransactionEvent
{
    public string RequestedBy { get; private set; }
    public Transaction Transaction { get; private set; }

    public TransactionRequestedEvent(Transaction transaction, string requestedBy)
    : base($"Requested by {requestedBy}")
    {
        RequestedBy = requestedBy;
        Transaction = transaction;
    }
}
