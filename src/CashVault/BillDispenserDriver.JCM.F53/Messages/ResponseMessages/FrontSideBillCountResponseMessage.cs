using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.ResponseMessages
{
    internal class FrontSideBillCountResponseMessage : BillCountResponseMessage
    {

        public FrontSideBillCountResponseMessage(byte[] data) : base(data)
        {

            // DH1 check
            if (data[1] != 0x03)
            {
                throw new ArgumentException("Invalid DH1");
            }
            DH1 = data[1];

        }
    }
}


