using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate.Events
{
    public class ShipmentReceivedEvent : TransactionEvent
    {
        public ParcelLockerShipment Shipment { get; set; }

        public ShipmentReceivedEvent(ParcelLockerShipment shipment)
            : base("Shipment received.")
        {
            Shipment = shipment;
        }
    }
}
