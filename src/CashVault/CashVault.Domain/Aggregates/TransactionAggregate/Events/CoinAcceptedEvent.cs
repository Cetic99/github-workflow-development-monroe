using CashVault.Domain.Common.Events;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.TransactionAggregate.Events;

public class CoinAcceptedEvent : BaseEvent
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; } = Currency.Default;

    public CoinAcceptedEvent(decimal amount, Currency? currency = null)
        : base($"Coin accepted: amount: {amount}, currency: {currency?.Code ?? Currency.Default.Code}")
    {
        Amount = amount;
        if (currency != null)
        {
            Currency = currency;
        }
    }
}
