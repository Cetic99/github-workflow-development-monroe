using System;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.OperatorAggregate.Events;

public class OperatorAuthenticationFailedEvent : DeviceEvent
{
    public Guid? CardUuid { get; private set; }
    public string? Username { get; private set; }

    public OperatorAuthenticationFailedEvent(Guid? cardUuid = null, string? username = null)
        : base($"Authentication failed", CashVault.Domain.Aggregates.DeviceAggregate.DeviceType.General)
    {
        CardUuid = cardUuid;
        Username = username;
    }
}
