using CashVault.ParcelLockerDriver.Enums;

namespace CashVault.ParcelLockerDriver.Messages.RequestMessages;

/// <summary>
/// Request to set the baud rate of the controller board
/// Command format: 0x9A [Address] [BaudRate] 0x77 [Checksum]
/// </summary>
public class SetBaudRateRequest : BaseRequestMessage
{
    private const byte FRAME_HEADER = 0x9A;
    private const byte COMMAND = 0x77;

    /// <summary>
    /// Board address (0x01 to 0x40, max 64 boards)
    /// </summary>
    public byte BoardAddress { get; set; }

    /// <summary>
    /// Baud rate to set
    /// </summary>
    public BaudRate BaudRate { get; set; }

    public SetBaudRateRequest(byte boardAddress, BaudRate baudRate) : base()
    {
        if (boardAddress < 0x01 || boardAddress > 0x40)
            throw new ArgumentOutOfRangeException(nameof(boardAddress), "Board address must be between 0x01 and 0x40");

        BoardAddress = boardAddress;
        BaudRate = baudRate;
    }

    public override byte[] GetMessageBytes()
    {
        byte[] messageData = new byte[]
        {
            FRAME_HEADER,
            BoardAddress,
            (byte)BaudRate,
            COMMAND
        };

        byte checksum = CalculateChecksum(messageData);

        return new byte[]
        {
            messageData[0],
            messageData[1],
            messageData[2],
            messageData[3],
            checksum
        };
    }
}
