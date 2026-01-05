using CashVault.Application.Common;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.TransactionAggregate.Events;

public class TicketAcceptedEvent : DeviceEvent, ISynchronizationEvent
{
    public string Barcode { get; init; }

    public TicketAcceptedEvent(string barcode)
        : base($"Ticket accepted [barcode: {barcode}")
    {
        Barcode = barcode;
    }
}
