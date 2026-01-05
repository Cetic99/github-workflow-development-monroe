using CashVault.Domain.Common.Events;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.TransactionAggregate.Events;

public class BillAcceptedEvent : TransactionEvent
{
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; } = Currency.Default;

    public BillAcceptedEvent(decimal amount, Currency? currency = null)
        : base($"Bill accepted, amount: {amount}, currency: {currency?.Code ?? Currency.Default.Code}")
    {
        Amount = amount;
        if (currency != null)
        {
            Currency = currency;
        }
    }

    public BillAcceptedEvent()
    {

    }
}
