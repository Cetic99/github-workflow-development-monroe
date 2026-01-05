using System;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class CardAuthenticatedEvent : DeviceEvent
{
    public Guid CardUuid { get; private set; }
    public string CardUID { get; private set; }
    public CardAuthenticatedEvent(Guid cardUuid, string cardUid)
        : base("card authenticated", DeviceAggregate.DeviceType.UserCardReader)
    {
        CardUuid = cardUuid;
        CardUID = cardUid;
    }
}
