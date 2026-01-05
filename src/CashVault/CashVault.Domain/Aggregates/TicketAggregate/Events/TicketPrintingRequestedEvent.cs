using CashVault.Application.Common;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.TicketAggregate.Events
{
    public class TicketPrintingRequestedEvent : TransactionEvent, ISynchronizationEvent
    {
        public TicketPrintingRequestedEvent(Ticket ticket)
            : base($"Ticket [{ticket.Number}] printing requested")
        {
            Ticket = ticket;
        }

        public Ticket Ticket { get; private set; }
    }
}
