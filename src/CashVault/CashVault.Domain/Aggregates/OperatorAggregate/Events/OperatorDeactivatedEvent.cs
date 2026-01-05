using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.OperatorAggregate.Events
{
    public class OperatorDeactivatedEvent : DeviceEvent
    {
        public OperatorDeactivatedEvent(Operator @operator)
             : base($"Operator {@operator.GetFullName()} deactivated", CashVault.Domain.Aggregates.DeviceAggregate.DeviceType.General)
        {
            Operator = @operator;
        }

        public Operator Operator { get; private set; }
    }
}
