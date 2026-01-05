using CashVault.ParcelLockerDriver.Interfaces;

namespace CashVault.ParcelLockerDriver.Messages.ResponseMessages;

/// <summary>
/// Base class for all parcel locker response messages
/// </summary>
public abstract class BaseResponseMessage : IParcelLockerResponse
{
    protected byte[] _data;
    public DateTime TimeStamp { get; protected set; }

    protected BaseResponseMessage(byte[] data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        TimeStamp = DateTime.UtcNow;
    }

    public byte[] GetMessageBytes()
    {
        return _data;
    }

    /// <summary>
    /// Verifies the XOR checksum of the response
    /// </summary>
    /// <returns>True if checksum is valid, false otherwise</returns>
    public bool VerifyChecksum()
    {
        if (_data.Length < 2)
            return false;

        byte calculatedChecksum = 0x00;
        for (int i = 0; i < _data.Length - 1; i++)
        {
            calculatedChecksum ^= _data[i];
        }

        return calculatedChecksum == _data[_data.Length - 1];
    }

    public abstract bool IsValidMessage();

    public override string ToString()
    {
        return BitConverter.ToString(GetMessageBytes()).Replace("-", " ");
    }
}
