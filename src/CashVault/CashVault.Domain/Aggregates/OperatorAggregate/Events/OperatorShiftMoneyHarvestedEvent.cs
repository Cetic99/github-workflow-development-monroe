using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.OperatorAggregate.Events
{
    public class OperatorShiftMoneyHarvestedEvent : TransactionEvent
    {
        public OperatorShiftMoneyHarvestedEvent(Operator @operator)
            : base($"Operator {@operator.GetFullName()} harvested shift money")
        {
            Operator = @operator;
        }

        public Operator Operator { get; private set; }
    }
}
