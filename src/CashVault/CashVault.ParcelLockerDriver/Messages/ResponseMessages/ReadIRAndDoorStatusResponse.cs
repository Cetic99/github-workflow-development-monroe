using System.Collections;

namespace CashVault.ParcelLockerDriver.Messages.ResponseMessages;

/// <summary>
/// Response to ReadIRAndDoorStatusRequest for a group of cabinets
/// Response format: 0x80 [Address] [DoorCh25-32] [DoorCh17-24] [DoorCh9-16] [DoorCh1-8] 
///                  [IRCh25-32] [IRCh17-24] [IRCh9-16] [IRCh1-8] 0x55 [Checksum]
/// First 4 bytes (after address) are door status: bit=1 closed, bit=0 open
/// Next 4 bytes are IR status: bit=1 unoccupied, bit=0 occupied
/// </summary>
public class ReadIRAndDoorStatusResponse : BaseResponseMessage
{
    private const byte FRAME_HEADER = 0x80;
    private const byte END_COMMAND = 0x55;
    private const int EXPECTED_LENGTH = 12;

    /// <summary>
    /// Board address (0x01 to 0x40)
    /// </summary>
    public byte BoardAddress { get; private set; }

    // Door status bytes
    public byte DoorCabinets25_32 { get; private set; }
    public byte DoorCabinets17_24 { get; private set; }
    public byte DoorCabinets9_16 { get; private set; }
    public byte DoorCabinets1_8 { get; private set; }

    // IR status bytes
    public byte IRCabinets25_32 { get; private set; }
    public byte IRCabinets17_24 { get; private set; }
    public byte IRCabinets9_16 { get; private set; }
    public byte IRCabinets1_8 { get; private set; }

    public ReadIRAndDoorStatusResponse(byte[] data) : base(data)
    {
        if (data.Length != EXPECTED_LENGTH)
            throw new ArgumentException($"Invalid data length. Expected {EXPECTED_LENGTH}, got {data.Length}");

        if (data[0] != FRAME_HEADER)
            throw new ArgumentException("Invalid frame header");

        if (data[10] != END_COMMAND)
            throw new ArgumentException("Invalid end command");

        BoardAddress = data[1];
        
        // Door status
        DoorCabinets25_32 = data[2];
        DoorCabinets17_24 = data[3];
        DoorCabinets9_16 = data[4];
        DoorCabinets1_8 = data[5];

        // IR status
        IRCabinets25_32 = data[6];
        IRCabinets17_24 = data[7];
        IRCabinets9_16 = data[8];
        IRCabinets1_8 = data[9];
    }

    /// <summary>
    /// Gets the door status for a specific cabinet (1-32)
    /// </summary>
    /// <param name="cabinetNumber">Cabinet number (1-32)</param>
    /// <returns>True if closed, false if open</returns>
    public bool IsCabinetDoorClosed(int cabinetNumber)
    {
        if (cabinetNumber < 1 || cabinetNumber > 32)
            throw new ArgumentOutOfRangeException(nameof(cabinetNumber), "Cabinet number must be between 1 and 32");

        byte statusByte;
        int bitPosition;

        if (cabinetNumber <= 8)
        {
            statusByte = DoorCabinets1_8;
            bitPosition = cabinetNumber - 1;
        }
        else if (cabinetNumber <= 16)
        {
            statusByte = DoorCabinets9_16;
            bitPosition = cabinetNumber - 9;
        }
        else if (cabinetNumber <= 24)
        {
            statusByte = DoorCabinets17_24;
            bitPosition = cabinetNumber - 17;
        }
        else
        {
            statusByte = DoorCabinets25_32;
            bitPosition = cabinetNumber - 25;
        }

        BitArray bits = new BitArray(new byte[] { statusByte });
        return bits[bitPosition]; // 1 = closed, 0 = open
    }

    /// <summary>
    /// Gets the occupancy status for a specific cabinet (1-32)
    /// </summary>
    /// <param name="cabinetNumber">Cabinet number (1-32)</param>
    /// <returns>True if unoccupied, false if occupied</returns>
    public bool IsCabinetUnoccupied(int cabinetNumber)
    {
        if (cabinetNumber < 1 || cabinetNumber > 32)
            throw new ArgumentOutOfRangeException(nameof(cabinetNumber), "Cabinet number must be between 1 and 32");

        byte statusByte;
        int bitPosition;

        if (cabinetNumber <= 8)
        {
            statusByte = IRCabinets1_8;
            bitPosition = cabinetNumber - 1;
        }
        else if (cabinetNumber <= 16)
        {
            statusByte = IRCabinets9_16;
            bitPosition = cabinetNumber - 9;
        }
        else if (cabinetNumber <= 24)
        {
            statusByte = IRCabinets17_24;
            bitPosition = cabinetNumber - 17;
        }
        else
        {
            statusByte = IRCabinets25_32;
            bitPosition = cabinetNumber - 25;
        }

        BitArray bits = new BitArray(new byte[] { statusByte });
        return bits[bitPosition]; // 1 = unoccupied, 0 = occupied
    }

    public override bool IsValidMessage()
    {
        return VerifyChecksum() && 
               _data[0] == FRAME_HEADER && 
               _data[10] == END_COMMAND;
    }
}
