using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate.Events
{
    public class ShipmentCollectedEvent : TransactionEvent
    {
        public ParcelLockerShipment Shipment { get; set; }

        public ShipmentCollectedEvent(ParcelLockerShipment shipment)
            : base("Shipment collected.")
        {
            Shipment = shipment;
        }
    }
}
