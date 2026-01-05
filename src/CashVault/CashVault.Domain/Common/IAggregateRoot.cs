using CashVault.Domain.Common.Events;
using System.Collections.Generic;

namespace CashVault.Domain;

public interface IAggregateRoot
{
    List<BaseEvent> DomainEvents { get; }
    void AddDomainEvent(BaseEvent domainEvent);
    void ClearDomainEvents();
}
