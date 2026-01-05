namespace CashVault.ParcelLockerDriver.Messages.RequestMessages;

/// <summary>
/// Request to open a specific cabinet
/// Command format: 0x8A [Address] [CabinetNo] 0x11 [Checksum]
/// The lock will take 1s to open and then there is feedback result
/// </summary>
public class OpenCabinetRequest : BaseRequestMessage
{
    private const byte FRAME_HEADER = 0x8A;
    private const byte COMMAND = 0x11;

    /// <summary>
    /// Board address (0x01 to 0x40, max 64 boards)
    /// </summary>
    public byte BoardAddress { get; set; }

    /// <summary>
    /// Cabinet number (0x01 to 0x18, from 1 to 24)
    /// </summary>
    public byte CabinetNumber { get; set; }

    public OpenCabinetRequest(byte boardAddress, byte cabinetNumber) : base()
    {
        if (boardAddress < 0x01 || boardAddress > 0x40)
            throw new ArgumentOutOfRangeException(nameof(boardAddress), "Board address must be between 0x01 and 0x40");

        if (cabinetNumber < 0x01 || cabinetNumber > 0x18)
            throw new ArgumentOutOfRangeException(nameof(cabinetNumber), "Cabinet number must be between 0x01 and 0x18 (1-24)");

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
