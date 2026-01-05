using CashVault.Application.Common;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.TicketAggregate.Events
{
    public class TicketRedeemedEvent : TransactionEvent, ISynchronizationEvent
    {
        public TicketRedeemedEvent(Ticket ticket)
            : base($"Ticket [{ticket.Number}] redeemed")
        {
            Ticket = ticket;
        }

        public Ticket Ticket { get; }
    }
}
