using CashVault.DeviceDriver.Common;

namespace CashVault.ParcelLockerDriver.Interfaces;

/// <summary>
/// Interface for parcel locker response messages
/// </summary>
public interface IParcelLockerResponse : ISerialPortMessage
{
    /// <summary>
    /// Validates the response message
    /// </summary>
    /// <returns>True if the message is valid, false otherwise</returns>
    bool IsValidMessage();

    /// <summary>
    /// Verifies the checksum of the response
    /// </summary>
    /// <returns>True if checksum is valid, false otherwise</returns>
    bool VerifyChecksum();
}
