namespace CashVault.ParcelLockerDriver.Enums;

/// <summary>
/// Occupancy status detected by IR sensor
/// </summary>
public enum OccupancyStatus : byte
{
    /// <summary>
    /// The cabinet is unoccupied (empty)
    /// </summary>
    Unoccupied = 0xEE,

    /// <summary>
    /// The cabinet is occupied (contains an item)
    /// </summary>
    Occupied = 0xFF
}
