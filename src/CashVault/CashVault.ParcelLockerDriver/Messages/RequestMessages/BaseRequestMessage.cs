using CashVault.ParcelLockerDriver.Interfaces;

namespace CashVault.ParcelLockerDriver.Messages.RequestMessages;

/// <summary>
/// Base class for all parcel locker request messages
/// </summary>
public abstract class BaseRequestMessage : IParcelLockerRequest
{
    public DateTime TimeStamp { get; protected set; }

    protected BaseRequestMessage()
    {
        TimeStamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the byte array representation of the message
    /// </summary>
    public abstract byte[] GetMessageBytes();

    /// <summary>
    /// Calculates XOR checksum for the message bytes
    /// </summary>
    /// <param name="data">Byte array to calculate checksum for (excluding the checksum byte itself)</param>
    /// <returns>XOR checksum byte</returns>
    public byte CalculateChecksum(byte[] data)
    {
        byte checksum = 0x00;
        foreach (byte b in data)
        {
            checksum ^= b;
        }
        return checksum;
    }

    public override string ToString()
    {
        return BitConverter.ToString(GetMessageBytes()).Replace("-", " ");
    }
}
