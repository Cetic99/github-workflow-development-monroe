using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.ResponseMessages
{
    internal class CancelResponseMessage : EnhancedResponseMessage
    {
        public CancelResponseMessage(byte[] data) : base(data)
        {
            //Data length check
            if (data.Length != 91)
            {
                throw new ArgumentException("Invalid data length!");
            }

            //DH1 check
            if (data[1] != 0x10)
            {
                throw new ArgumentException("Invalid DH1");
            }
            DH1 = data[1];

            //DH3 check
            if (data[4] != 0x00 || data[5] != 0x54)
            {
                throw new ArgumentException("Invalid DH3");
            }
            DH3 = [data[4], data[5]];
        }

        public override bool IsValidMessage()
        {
            return true;
        }
    }
}
