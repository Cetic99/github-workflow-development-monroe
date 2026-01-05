using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.OperatorAggregate.Events
{
    public class OperatorCreatedEvent : DeviceEvent
    {
        public OperatorCreatedEvent(Operator newOperator)
            : base($"Operator {newOperator.GetFullName()} created", CashVault.Domain.Aggregates.DeviceAggregate.DeviceType.General)
        {
            Operator = newOperator;
        }

        public Operator Operator { get; private set; }
    }
}
