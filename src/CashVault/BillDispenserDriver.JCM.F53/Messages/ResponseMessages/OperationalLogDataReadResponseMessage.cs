using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.ResponseMessages
{
    internal class OperationalLogDataReadResponseMessage : EnhancedResponseMessage
    {
        public OperationalLogDataReadResponseMessage(byte[] data) : base(data)
        {
            //Data length check
            if (data.Length != 1115)
            {
                throw new ArgumentException("Invalid data length!");
            }

            //DH1 check
            if (data[1] != 0x13)
            {
                throw new ArgumentException("Invalid DH1");
            }
            DH1 = data[1];

            //DH3 check
            if (data[4] != 0x04 || data[5] != 0x54)
            {
                throw new ArgumentException("Invalid DH3");
            }
            DH3 = [data[4], data[5]];

            //Operational log data
            byte[] OperationalLogData = data.Skip(90).ToArray();
        }

        public override bool IsValidMessage()
        {
            return true;
        }
    }
}
