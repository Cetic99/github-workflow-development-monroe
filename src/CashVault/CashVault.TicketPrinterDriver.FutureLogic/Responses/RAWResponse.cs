using CashVault.DeviceDriver.Common;

namespace CashVault.TicketPrinterDriver.FutureLogic.Responses
{
    public class RAWResponse : ISerialPortMessage
    {
        public byte[] _data;

        public RAWResponse(byte[] data)
        {
            _data = data;
        }
        public DateTime TimeStamp
        {
            get { return DateTime.UtcNow; }
        }

        public byte[] GetMessageBytes()
        {
            return _data;
        }
    }
}