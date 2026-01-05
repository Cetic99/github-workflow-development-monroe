using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.OperatorAggregate.Events
{
    public class OperatorActivatedEvent : DeviceEvent
    {
        public OperatorActivatedEvent(Operator @operator)
            : base($"Operator {@operator.GetFullName()} activated", CashVault.Domain.Aggregates.DeviceAggregate.DeviceType.General)
        {
            Operator = @operator;
        }

        public Operator Operator { get; }
    }
}
