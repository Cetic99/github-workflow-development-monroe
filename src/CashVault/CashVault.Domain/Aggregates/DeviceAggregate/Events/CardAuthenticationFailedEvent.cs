using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class CardAuthenticationFailedEvent : DeviceFailEvent
{
    public CardAuthenticationFailedEvent(string reason)
        : base($"card authentication failed with reason: {reason}", DeviceAggregate.DeviceType.UserCardReader)
    {
    }
}
