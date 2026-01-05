using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate;

public class ShipmentStatus : Enumeration
{
    /// <summary>
    /// Shipment is manually created (by sender) but not yet stored in some parcel locker.
    /// </summary>
    public static ShipmentStatus Draft { get; } = new(1, nameof(Draft));

    /// <summary>
    /// Shipment is received and registered in the parcel locker.
    /// </summary>
    public static ShipmentStatus Received { get; } = new(2, nameof(Received));

    /// <summary>
    /// Expired shipment has beed picked up by courier.
    /// </summary>
    public static ShipmentStatus Returned { get; } = new(3, nameof(Returned));

    /// <summary>
    /// Shipment is picked up by the recipient or courier.
    /// </summary>
    public static ShipmentStatus Collected { get; } = new(4, nameof(Collected)); // picked up, accepted

    /// <summary>
    /// Shipment is cancelled by sender or system.
    /// </summary>
    public static ShipmentStatus Cancelled { get; } = new(5, nameof(Cancelled));

    /// <summary>
    /// Shipment is expired (not picked before its expiration time).
    /// </summary>
    public static ShipmentStatus Expired { get; } = new(6, nameof(Expired));

    /// <summary>
    /// Shipment is announced (by system) but not yet received in parcel locker.
    /// </summary>
    public static ShipmentStatus Announced { get; } = new(7, nameof(Announced));

    public ShipmentStatus(int id, string code)
        : base(id, code) { }
}
