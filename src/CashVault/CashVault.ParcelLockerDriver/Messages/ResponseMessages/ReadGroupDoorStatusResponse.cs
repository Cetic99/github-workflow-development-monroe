using System.Collections;

namespace CashVault.ParcelLockerDriver.Messages.ResponseMessages;

/// <summary>
/// Response to ReadDoorStatusRequest for a group of cabinets
/// Response format: 0x80 [Address] [Ch25-32] [Ch17-24] [Ch9-16] [Ch1-8] 0x33 [Checksum]
/// Each byte represents 8 cabinets, where bit=1 means closed and bit=0 means open
/// </summary>
public class ReadGroupDoorStatusResponse : BaseResponseMessage
{
    private const byte FRAME_HEADER = 0x80;
    private const byte END_COMMAND = 0x33;
    private const int EXPECTED_LENGTH = 8;

    /// <summary>
    /// Board address (0x01 to 0x40)
    /// </summary>
    public byte BoardAddress { get; private set; }

    /// <summary>
    /// Status of cabinets 25-32 (bit 0-7 from low to high)
    /// </summary>
    public byte Cabinets25_32 { get; private set; }

    /// <summary>
    /// Status of cabinets 17-24 (bit 0-7 from low to high)
    /// </summary>
    public byte Cabinets17_24 { get; private set; }

    /// <summary>
    /// Status of cabinets 9-16 (bit 0-7 from low to high)
    /// </summary>
    public byte Cabinets9_16 { get; private set; }

    /// <summary>
    /// Status of cabinets 1-8 (bit 0-7 from low to high)
    /// </summary>
    public byte Cabinets1_8 { get; private set; }

    public ReadGroupDoorStatusResponse(byte[] data) : base(data)
    {
        if (data.Length != EXPECTED_LENGTH)
            throw new ArgumentException($"Invalid data length. Expected {EXPECTED_LENGTH}, got {data.Length}");

        if (data[0] != FRAME_HEADER)
            throw new ArgumentException("Invalid frame header");

        if (data[6] != END_COMMAND)
            throw new ArgumentException("Invalid end command");

        BoardAddress = data[1];
        Cabinets25_32 = data[2];
        Cabinets17_24 = data[3];
        Cabinets9_16 = data[4];
        Cabinets1_8 = data[5];
    }

    /// <summary>
    /// Gets the door status for a specific cabinet (1-32)
    /// </summary>
    /// <param name="cabinetNumber">Cabinet number (1-32)</param>
    /// <returns>True if closed, false if open</returns>
    public bool IsCabinetClosed(int cabinetNumber)
    {
        if (cabinetNumber < 1 || cabinetNumber > 32)
            throw new ArgumentOutOfRangeException(nameof(cabinetNumber), "Cabinet number must be between 1 and 32");

        byte statusByte;
        int bitPosition;

        if (cabinetNumber <= 8)
        {
            statusByte = Cabinets1_8;
            bitPosition = cabinetNumber - 1;
        }
        else if (cabinetNumber <= 16)
        {
            statusByte = Cabinets9_16;
            bitPosition = cabinetNumber - 9;
        }
        else if (cabinetNumber <= 24)
        {
            statusByte = Cabinets17_24;
            bitPosition = cabinetNumber - 17;
        }
        else
        {
            statusByte = Cabinets25_32;
            bitPosition = cabinetNumber - 25;
        }

        BitArray bits = new BitArray(new byte[] { statusByte });
        return bits[bitPosition]; // 1 = closed, 0 = open
    }

    public override bool IsValidMessage()
    {
        return VerifyChecksum() && 
               _data[0] == FRAME_HEADER && 
               _data[6] == END_COMMAND;
    }
}
