using CashVault.Domain.Common.Events;
using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.TransactionAggregate.Events;

public class TransactionCompletedEvent : TransactionEvent
{
    public Transaction Transaction { get; private set; }

    public TransactionCompletedEvent(Transaction transaction)
        : base($"Transaction completed, amount: {transaction.Amount}, currency: {transaction.Currency.Code ?? Currency.Default.Code}")
    {
        Transaction = transaction;
    }
}
