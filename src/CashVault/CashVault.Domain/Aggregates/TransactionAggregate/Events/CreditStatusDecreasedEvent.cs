using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.TransactionAggregate.Events
{
    public class CreditStatusDecreasedEvent : TransactionEvent
    {
        public decimal Amount { get; private set; }
        public CreditStatusDecreasedEvent(decimal amount)
            : base($"Credit status decreased for amount: {amount}")
        {
            Amount = amount;
        }
    }
}
