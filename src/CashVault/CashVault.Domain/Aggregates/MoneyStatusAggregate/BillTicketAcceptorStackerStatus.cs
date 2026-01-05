using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;
using CashVault.Domain.Aggregates.TicketAggregate;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate;

public class BillTicketAcceptorStackerStatus : MoneyStatus, ICloneable
{
    public int BillCount { get; private set; }
    public decimal BillAmount { get; private set; }
    public int TicketCount { get; private set; }
    public decimal TicketAmount { get; private set; }

    public BillTicketAcceptorStackerStatus()
    {
        BillCount = 0;
        BillAmount = 0;
        TicketCount = 0;
        TicketAmount = 0;
    }

    [JsonConstructor]
    public BillTicketAcceptorStackerStatus(
        int billCount,
        decimal billAmount,
        int ticketCount,
        decimal ticketAmount)
    {
        BillCount = billCount;
        BillAmount = billAmount;
        TicketCount = ticketCount;
        TicketAmount = ticketAmount;
    }

    public void AddBill(int count, decimal amount)
    {
        var oldStatus = (BillTicketAcceptorStackerStatus)this.Clone();

        BillCount += count;
        BillAmount += amount;

        var newStatus = (BillTicketAcceptorStackerStatus)this.Clone();

        AddDomainEvent(new BillAddedToAcceptorStackerEvent(oldStatus, newStatus));
    }

    public void AddTicket(Ticket ticket)
    {
        var oldStatus = (BillTicketAcceptorStackerStatus)this.Clone();

        TicketCount += 1;
        TicketAmount += ticket.Amount;

        var newStatus = (BillTicketAcceptorStackerStatus)this.Clone();

        AddDomainEvent(new TicketAddedToAcceptorStackerEvent(oldStatus, newStatus));
    }

    /// <summary>
    /// Add ticket redeemed from CMS
    /// </summary>
    public void AddTicket(decimal amount)
    {
        var oldStatus = (BillTicketAcceptorStackerStatus)this.Clone();

        TicketCount += 1;
        TicketAmount += amount;

        var newStatus = (BillTicketAcceptorStackerStatus)this.Clone();

        AddDomainEvent(new TicketAddedToAcceptorStackerEvent(oldStatus, newStatus));
    }

    public void Empty()
    {
        var oldStatus = (BillTicketAcceptorStackerStatus)this.Clone();

        BillCount = 0;
        BillAmount = 0;
        TicketCount = 0;
        TicketAmount = 0;

        var newStatus = (BillTicketAcceptorStackerStatus)this.Clone();

        AddDomainEvent(new AcceptorEmptiedEvent(oldStatus, newStatus));
    }

    public override void ToJsonString()
    {
        JsonValue = JsonSerializer.Serialize(this);
    }

    public override void Initialize()
    {
        if (JsonValue == null) return;

        BillTicketAcceptorStackerStatus? obj = JsonSerializer.Deserialize<BillTicketAcceptorStackerStatus>(JsonValue);

        if (obj != null)
        {
            BillCount = obj.BillCount;
            BillAmount = obj.BillAmount;
            TicketCount = obj.TicketCount;
            TicketAmount = obj.TicketAmount;
        }
    }

    public object Clone()
    {
        var json = JsonSerializer.Serialize(this);

        return JsonSerializer.Deserialize<BillTicketAcceptorStackerStatus>(json)
            ?? new();
    }
}