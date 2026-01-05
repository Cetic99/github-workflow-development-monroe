namespace CashVault.ParcelLockerDriver.Enums;

/// <summary>
/// Lock status of a cabinet door
/// </summary>
public enum LockStatus : byte
{
    /// <summary>
    /// The cabinet door is closed/locked
    /// </summary>
    Closed = 0x00,

    /// <summary>
    /// The cabinet door is open/unlocked
    /// </summary>
    Open = 0x11
}
