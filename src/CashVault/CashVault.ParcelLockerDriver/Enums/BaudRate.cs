namespace CashVault.ParcelLockerDriver.Enums;

/// <summary>
/// Supported baud rates for the parcel locker controller
/// </summary>
public enum BaudRate : byte
{
    /// <summary>
    /// 9600 baud
    /// </summary>
    Baud9600 = 0x96,

    /// <summary>
    /// 14400 baud
    /// </summary>
    Baud14400 = 0x14,

    /// <summary>
    /// 19200 baud
    /// </summary>
    Baud19200 = 0x19,

    /// <summary>
    /// 38400 baud
    /// </summary>
    Baud38400 = 0x38,

    /// <summary>
    /// 57600 baud
    /// </summary>
    Baud57600 = 0x57,

    /// <summary>
    /// 115200 baud
    /// </summary>
    Baud115200 = 0x11
}
