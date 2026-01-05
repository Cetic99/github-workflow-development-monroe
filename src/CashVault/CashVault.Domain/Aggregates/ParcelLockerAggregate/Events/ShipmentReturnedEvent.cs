using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate.Events
{
    public class ShipmentReturnedEvent : TransactionEvent
    {
        public ParcelLockerShipment Shipment { get; set; }

        public ShipmentReturnedEvent(ParcelLockerShipment shipment)
            : base("Shipment retrned.")
        {
            Shipment = shipment;
        }
    }
}
