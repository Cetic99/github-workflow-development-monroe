using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.OperatorAggregate.Events
{
    public class OperatorPasswordUpdatedEvent : DeviceEvent
    {
        public OperatorPasswordUpdatedEvent(Operator @operator)
            : base($"Operator: {@operator.GetFullName()} password updated", CashVault.Domain.Aggregates.DeviceAggregate.DeviceType.General)
        {
            Operator = @operator;
        }

        public Operator Operator { get; private set; }
    }
}
