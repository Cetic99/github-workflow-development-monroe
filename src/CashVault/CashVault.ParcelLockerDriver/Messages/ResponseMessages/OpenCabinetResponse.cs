using CashVault.ParcelLockerDriver.Enums;

namespace CashVault.ParcelLockerDriver.Messages.ResponseMessages;

/// <summary>
/// Response to OpenCabinetRequest
/// Response format: 0x8A [Address] [CabinetNo] [Status] [Checksum]
/// Returns two responses: first when opened (0x11), then when closed (0x00)
/// </summary>
public class OpenCabinetResponse : BaseResponseMessage
{
    private const byte FRAME_HEADER = 0x8A;
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
    /// Lock status (0x11 = open, 0x00 = closed)
    /// </summary>
    public LockStatus LockStatus { get; private set; }

    /// <summary>
    /// Indicates if the cabinet is currently open
    /// </summary>
    public bool IsOpen => LockStatus == LockStatus.Open;

    public OpenCabinetResponse(byte[] data) : base(data)
    {
        if (data.Length != EXPECTED_LENGTH)
            throw new ArgumentException($"Invalid data length. Expected {EXPECTED_LENGTH}, got {data.Length}");

        if (data[0] != FRAME_HEADER)
            throw new ArgumentException("Invalid frame header");

        BoardAddress = data[1];
        CabinetNumber = data[2];
        LockStatus = (LockStatus)data[3];
    }

    public override bool IsValidMessage()
    {
        return VerifyChecksum() && 
               _data[0] == FRAME_HEADER &&
               (LockStatus == LockStatus.Open || LockStatus == LockStatus.Closed);
    }
}
