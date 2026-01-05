using System;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class CardEnrolledEvent : DeviceEvent
{
    public Guid CardUuid { get; set; }
    public CardEnrolledEvent(Guid cardUuid)
        : base("card enrolled", DeviceAggregate.DeviceType.UserCardReader)
    {
        CardUuid = cardUuid;
    }
}