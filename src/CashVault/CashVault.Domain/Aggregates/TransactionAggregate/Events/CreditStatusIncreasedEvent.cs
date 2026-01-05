using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.TransactionAggregate.Events
{
    public class CreditStatusIncreasedEvent : TransactionEvent
    {
        public decimal Amount { get; private set; }
        public CreditStatusIncreasedEvent(decimal amount)
            : base($"Credit status increased for amount: {amount}")
        {
            Amount = amount;
        }
    }
}
