using CashVault.ParcelLockerDriver.Enums;

namespace CashVault.ParcelLockerDriver.Messages.ResponseMessages;

/// <summary>
/// Response to ReadIRStatusRequest for a single cabinet
/// Response format: 0x80 [Address] [CabinetNo] [Status] [Checksum]
/// </summary>
public class ReadSingleIRStatusResponse : BaseResponseMessage
{
    private const byte FRAME_HEADER = 0x80;
    private const int EXPECTED_LENGTH = 5;

    /// <summary>
    /// Board address (0x01 to 0x40)
    /// </summary>
    public byte BoardAddress { get; private set; }

    /// <summary>
    /// Cabinet number (0x01 to 0x18)
    /// </summary>
    public byte CabinetNumber { get; private set; }

    /// <summary>
    /// Occupancy status (0xFF = occupied, 0xEE = unoccupied)
    /// </summary>
    public OccupancyStatus OccupancyStatus { get; private set; }

    /// <summary>
    /// Indicates if the cabinet is currently occupied
    /// </summary>
    public bool IsOccupied => OccupancyStatus == OccupancyStatus.Occupied;

    public ReadSingleIRStatusResponse(byte[] data) : base(data)
    {
        if (data.Length != EXPECTED_LENGTH)
            throw new ArgumentException($"Invalid data length. Expected {EXPECTED_LENGTH}, got {data.Length}");

        if (data[0] != FRAME_HEADER)
            throw new ArgumentException("Invalid frame header");

        BoardAddress = data[1];
        CabinetNumber = data[2];
        OccupancyStatus = (OccupancyStatus)data[3];
    }

    public override bool IsValidMessage()
    {
        return VerifyChecksum() && 
               _data[0] == FRAME_HEADER &&
               (OccupancyStatus == OccupancyStatus.Occupied || OccupancyStatus == OccupancyStatus.Unoccupied);
    }
}
