using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.OperatorAggregate.Events
{
    public class OperatorCardAddedEvent : TransactionEvent
    {
        public OperatorCardAddedEvent(Operator @operator, IdentificationCard card)
            : base($"Card for operator: {@operator.GetFullName()} added")
        {
            Operator = @operator;
            Card = card;
        }

        public Operator Operator { get; private set; }
        public IdentificationCard Card { get; private set; }
    }
}
