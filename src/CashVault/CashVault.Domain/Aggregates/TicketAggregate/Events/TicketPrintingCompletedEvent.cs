using CashVault.Application.Common;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.TicketAggregate.Events
{
    public class TicketPrintingCompletedEvent : TransactionEvent, ISynchronizationEvent
    {
        public TicketPrintingCompletedEvent(Ticket ticket)
            : base($"Ticket [{ticket.Number}] printing completed")
        {
            Ticket = ticket;
        }

        public Ticket Ticket { get; private set; }
    }
}
