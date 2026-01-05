using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate.Events
{
    public class ShipmentExpiredEvent : TransactionEvent
    {
        public ParcelLockerShipment Shipment { get; set; }

        public ShipmentExpiredEvent(ParcelLockerShipment shipment)
            : base("Shipment expired")
        {
            Shipment = shipment;
        }
    }
}
