namespace CashVault.ParcelLockerDriver.Messages.ResponseMessages;

/// <summary>
/// Response to QueryBoardAddressRequest
/// Response format: 0x80 0x01 [Address] 0x99 [Checksum]
/// </summary>
public class QueryBoardAddressResponse : BaseResponseMessage
{
    private const byte FRAME_HEADER = 0x80;
    private const byte BYTE1 = 0x01;
    private const byte END_COMMAND = 0x99;
    private const int EXPECTED_LENGTH = 5;

    /// <summary>
    /// Board address (0x01 to 0x40)
    /// </summary>
    public byte BoardAddress { get; private set; }

    public QueryBoardAddressResponse(byte[] data) : base(data)
    {
        if (data.Length != EXPECTED_LENGTH)
            throw new ArgumentException($"Invalid data length. Expected {EXPECTED_LENGTH}, got {data.Length}");

        if (data[0] != FRAME_HEADER)
            throw new ArgumentException("Invalid frame header");

        if (data[1] != BYTE1)
            throw new ArgumentException("Invalid byte 1");

        if (data[3] != END_COMMAND)
            throw new ArgumentException("Invalid end command");

        BoardAddress = data[2];
    }

    public override bool IsValidMessage()
    {
        return VerifyChecksum() && 
               _data[0] == FRAME_HEADER && 
               _data[1] == BYTE1 && 
               _data[3] == END_COMMAND;
    }
}
