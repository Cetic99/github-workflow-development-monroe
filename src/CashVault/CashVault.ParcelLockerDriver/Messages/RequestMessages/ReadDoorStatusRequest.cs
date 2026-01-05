namespace CashVault.ParcelLockerDriver.Messages.RequestMessages;

/// <summary>
/// Request to read the door status of cabinet(s)
/// Command format: 0x80 [Address] [CabinetNo] 0x33 [Checksum]
/// If CabinetNo is 0x00, reads all cabinets in the group
/// If CabinetNo is 0x01-0x18, reads a single cabinet
/// </summary>
public class ReadDoorStatusRequest : BaseRequestMessage
{
    private const byte FRAME_HEADER = 0x80;
    private const byte COMMAND = 0x33;

    /// <summary>
    /// Board address (0x01 to 0x40, max 64 boards)
    /// </summary>
    public byte BoardAddress { get; set; }

    /// <summary>
    /// Cabinet number (0x00 for all, 0x01 to 0x18 for specific cabinet)
    /// </summary>
    public byte CabinetNumber { get; set; }

    /// <summary>
    /// Indicates if this request is for all cabinets in the group
    /// </summary>
    public bool IsGroupRequest => CabinetNumber == 0x00;

    public ReadDoorStatusRequest(byte boardAddress, byte cabinetNumber) : base()
    {
        if (boardAddress < 0x01 || boardAddress > 0x40)
            throw new ArgumentOutOfRangeException(nameof(boardAddress), "Board address must be between 0x01 and 0x40");

        if (cabinetNumber > 0x18)
            throw new ArgumentOutOfRangeException(nameof(cabinetNumber), "Cabinet number must be between 0x00 and 0x18");

        BoardAddress = boardAddress;
        CabinetNumber = cabinetNumber;
    }

    public override byte[] GetMessageBytes()
    {
        byte[] messageData = new byte[]
        {
            FRAME_HEADER,
            BoardAddress,
            CabinetNumber,
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
