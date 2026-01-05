using CashVault.BillDispenserDriver.JCM.F53.Interfaces;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.RequestMessages
{
    public class EnhancedRequestMessage : IBillDispenserRequest
    {
        public byte DH0 = 0x60;
        public byte DH2 = 0xFF;
        public byte FS = 0x1C;

        public bool IsEnhanced()
        {
            return true;
        }

        public virtual byte[] GetMessageBytes()
        {
            throw new NotImplementedException();
        }

        public DateTime TimeStamp
        {
            get { return DateTime.UtcNow; }
        }
    }
}