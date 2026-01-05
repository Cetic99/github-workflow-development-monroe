using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.OperatorAggregate.Events;

public class OperatorAuthenticatedEvent : DeviceEvent
{
    public Operator Operator { get; set; }

    public OperatorAuthenticatedEvent(Operator @operator)
        : base($"Operator {@operator.GetFullName()} authenticated", DeviceAggregate.DeviceType.General)
    {
        Operator = @operator;
    }
}
