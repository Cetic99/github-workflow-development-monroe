using CashVault.ParcelLockerDriver.Enums;

namespace CashVault.ParcelLockerDriver.Messages.ResponseMessages;

/// <summary>
/// Response to SetBaudRateRequest
/// Response format: 0x9A [Address] [BaudRate] 0x99 [Checksum]
/// </summary>
public class SetBaudRateResponse : BaseResponseMessage
{
    private const byte FRAME_HEADER = 0x9A;
    private const byte END_COMMAND = 0x99;
    private const int EXPECTED_LENGTH = 5;

    /// <summary>
    /// Board address (0x01 to 0x40)
    /// </summary>
    public byte BoardAddress { get; private set; }

    /// <summary>
    /// Configured baud rate
    /// </summary>
    public BaudRate BaudRate { get; private set; }

    public SetBaudRateResponse(byte[] data) : base(data)
    {
        if (data.Length != EXPECTED_LENGTH)
            throw new ArgumentException($"Invalid data length. Expected {EXPECTED_LENGTH}, got {data.Length}");

        if (data[0] != FRAME_HEADER)
            throw new ArgumentException("Invalid frame header");

        if (data[3] != END_COMMAND)
            throw new ArgumentException("Invalid end command");

        BoardAddress = data[1];
        BaudRate = (BaudRate)data[2];
    }

    public override bool IsValidMessage()
    {
        return VerifyChecksum() && 
               _data[0] == FRAME_HEADER && 
               _data[3] == END_COMMAND;
    }
}
