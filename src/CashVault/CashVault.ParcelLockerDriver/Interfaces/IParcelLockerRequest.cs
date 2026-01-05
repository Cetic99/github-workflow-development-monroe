using CashVault.DeviceDriver.Common;

namespace CashVault.ParcelLockerDriver.Interfaces;

/// <summary>
/// Interface for parcel locker request messages
/// </summary>
public interface IParcelLockerRequest : ISerialPortMessage
{
    /// <summary>
    /// Calculates XOR checksum for the message
    /// </summary>
    /// <param name="data">Byte array to calculate checksum for</param>
    /// <returns>XOR checksum byte</returns>
    byte CalculateChecksum(byte[] data);
}
