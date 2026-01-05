using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CashVault.Domain.Aggregates.TicketAggregate.Events;
using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.TicketAggregate;

public class Ticket : Entity, IAggregateRoot
{
    private List<BaseEvent> _domainEvents = [];
    public Guid Guid { get; protected set; }
    /// <summary>
    /// If ticket is created by local system, not on casino management system or some other external system
    /// </summary>
    public bool IsLocal { get; private set; }
    private int ticketTypeId { get; set; }
    public TicketType Type { get; private set; }
    public string Number { get; private set; }
    public string Barcode { get; private set; }
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }
    public bool IsPrinted { get; private set; } = false;

    /// <summary>
    /// Ticket is redeemed when it is only validated by ticket acceptor or scanned by ticket scanner
    /// </summary>
    public bool IsRedeemed { get; private set; } = false;
    /// <summary>
    /// Ticket is stacked when it is stacked in ticket acceptor or scanned by ticket scanner
    /// </summary>
    public bool IsStacked { get; private set; } = false;
    public DateTime PrintingRequestedAt { get; private set; }
    public DateTime? PrintingCompletedAt { get; private set; }
    public int DaysValid { get; private set; }
    public DateTime ExpirationDateTime { get; private set; }
    public bool IsExpired => DateTime.UtcNow > ExpirationDateTime;
    public bool IsValid => !IsExpired && IsPrinted;
    public bool CanBeRedeemed => IsValid && !IsRedeemed;

    [NotMapped]
    public List<BaseEvent> DomainEvents => _domainEvents ?? [];

    private Ticket()
    {
    }

    public Ticket(TicketType type, string barcode, bool isLocal, string number, decimal amount, int daysValid = 7, Currency? currency = null)
    {
        if (barcode.Length != 18)
        {
            throw new ArgumentException("Barcode must be 18 characters long.", nameof(barcode));
        }

        Guid = Guid.NewGuid();
        Type = type;
        Barcode = barcode;
        IsLocal = isLocal;
        Number = number;
        Amount = amount;
        Currency = currency ?? Currency.Default;
        DaysValid = daysValid;
        ExpirationDateTime = DateTime.UtcNow.AddDays(daysValid);
    }

    public bool Redeem()
    {
        if (!CanBeRedeemed)
        {
            return false;
        }

        IsRedeemed = true;
        AddDomainEvent(new TicketRedeemedEvent(this));
        return true;
    }

    public bool Stack()
    {
        if (IsStacked)
        {
            return false;
        }

        IsStacked = true;
        return true;
    }

    /// <summary>
    /// Request printing
    /// </summary>
    /// <param name="targetDateTime">If not passed DateTime.UtcNow used</param>
    public void RequestPrinting(DateTime? targetDateTime = null)
    {
        PrintingRequestedAt = targetDateTime ?? DateTime.UtcNow;
        IsPrinted = false;
        AddDomainEvent(new TicketPrintingRequestedEvent(this));
    }

    /// <summary>
    /// Complete printing
    /// </summary>
    /// <param name="targetDateTime">If not passed DateTime.UtcNow used</param>
    public void CompletePrinting(DateTime? targetDateTime = null)
    {
        PrintingCompletedAt = targetDateTime ?? DateTime.UtcNow;
        IsPrinted = true;
        AddDomainEvent(new TicketPrintingCompletedEvent(this));
    }

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents ??= [];
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
