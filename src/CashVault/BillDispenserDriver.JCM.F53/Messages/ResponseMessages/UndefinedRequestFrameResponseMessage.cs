using CashVault.BillDispenserDriver.JCM.F53.Interfaces;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.ResponseMessages
{
    public class UndefinedRequestFrameResponseMessage : IBillDispenserResponse
    {
        /* TODO: Check this message it is not positive/negative, it is fixed response. Check filtering in message factory if it does not implement IBillDispenserResponse Interface*/

        public byte DH0 = 0xF0;
        public byte DH1 = 0x00;
        public byte DH2 = 0x02;
        public byte undefinedDH0;
        public byte undefinedDH1;
        public byte FS = 0x1C;
        public DateTime TimeStamp => DateTime.UtcNow;

        public UndefinedRequestFrameResponseMessage(byte[] data)
        {
            if (data.Length != 6)
            {
                throw new ArgumentException("Invalid data length!");
            }

            if (data[0] != DH0 || data[1] != DH1 || data[2] != DH2 || data[5] != FS)
            {
                throw new ArgumentException("Invalid data");
            }
        }

        public byte[] GetMessageBytes()
        {
            throw new NotImplementedException();
        }

        public bool IsEnhanced()
        {
            throw new NotImplementedException();
        }

        public bool IsPositive()
        {
            throw new NotImplementedException();
        }

        public bool IsValidMessage()
        {
            return true;
        }
    }
}
