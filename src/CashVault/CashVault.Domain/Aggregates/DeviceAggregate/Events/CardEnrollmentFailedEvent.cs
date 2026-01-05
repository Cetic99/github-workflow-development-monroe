using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class CardEnrollmentFailedEvent : DeviceFailEvent
{
    public CardEnrollmentFailedEvent(string reason)
        : base($"card enrollment failed with reason: {reason}", DeviceAggregate.DeviceType.UserCardReader)
    {
    }
}
