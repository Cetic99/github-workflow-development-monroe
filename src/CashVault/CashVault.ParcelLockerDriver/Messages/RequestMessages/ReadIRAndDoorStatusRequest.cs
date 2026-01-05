namespace CashVault.ParcelLockerDriver.Messages.RequestMessages;

/// <summary>
/// Request to read both IR and door status of all cabinets in a group
/// Command format: 0x80 [Address] 0x00 0x55 [Checksum]
/// </summary>
public class ReadIRAndDoorStatusRequest : BaseRequestMessage
{
    private const byte FRAME_HEADER = 0x80;
    private const byte CABINET_NUMBER = 0x00; // Always 0x00 for group request
    private const byte COMMAND = 0x55;

    /// <summary>
    /// Board address (0x01 to 0x40, max 64 boards)
    /// </summary>
    public byte BoardAddress { get; set; }

    public ReadIRAndDoorStatusRequest(byte boardAddress) : base()
    {
        if (boardAddress < 0x01 || boardAddress > 0x40)
            throw new ArgumentOutOfRangeException(nameof(boardAddress), "Board address must be between 0x01 and 0x40");

        BoardAddress = boardAddress;
    }

    public override byte[] GetMessageBytes()
    {
        byte[] messageData = new byte[]
        {
            FRAME_HEADER,
            BoardAddress,
            CABINET_NUMBER,
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
