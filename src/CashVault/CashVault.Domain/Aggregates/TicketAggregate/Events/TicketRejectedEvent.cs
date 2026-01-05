using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.TicketAggregate.Events
{
    public class TicketRejectedEvent : DeviceFailEvent
    {
        public TicketRejectedEvent(string? message)
            : base(string.IsNullOrEmpty(message) ? $"Ticket rejected" : message, DeviceAggregate.DeviceType.BillAcceptor)
        {
        }
    }
}
