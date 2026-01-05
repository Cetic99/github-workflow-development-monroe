

using CashVault.BillDispenserDriver.JCM.F53.Messages.Common;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.RequestMessages
{
    public class AutomaticallyRejectBillCountRequestMessage : BillCountRequestMessage
    {
        public AutomaticallyRejectBillCountRequestMessage(BillCountConfiguration configuration)
            : base(configuration)
        {
            // Set DH1 to 0x11
            DH1 = 0x11;
        }
    }
}
