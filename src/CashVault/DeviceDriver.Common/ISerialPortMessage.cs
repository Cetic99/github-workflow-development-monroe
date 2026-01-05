namespace CashVault.DeviceDriver.Common;

/// <summary>
/// Represents a serial port message.
/// </summary>
public interface ISerialPortMessage
{
    /// <summary>
    /// Gets the byte array representation of the message.
    /// </summary>
    /// <returns>The byte array representing the message.</returns>
    byte[] GetMessageBytes();

    /// <summary>
    /// Gets the timestamp of the message.
    /// </summary>
    DateTime TimeStamp { get; }
}
