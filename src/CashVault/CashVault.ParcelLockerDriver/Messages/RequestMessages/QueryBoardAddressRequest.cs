namespace CashVault.ParcelLockerDriver.Messages.RequestMessages;

/// <summary>
/// Request to query the board address
/// Command format: 0x80 0x01 0x00 0x99 [Checksum]
/// Each query can check one board only
/// </summary>
public class QueryBoardAddressRequest : BaseRequestMessage
{
    private const byte FRAME_HEADER = 0x80;
    private const byte BYTE1 = 0x01;
    private const byte BYTE2 = 0x00;
    private const byte BYTE3 = 0x99;

    public QueryBoardAddressRequest() : base()
    {
    }

    public override byte[] GetMessageBytes()
    {
        byte[] messageData = new byte[]
        {
            FRAME_HEADER,
            BYTE1,
            BYTE2,
            BYTE3
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
