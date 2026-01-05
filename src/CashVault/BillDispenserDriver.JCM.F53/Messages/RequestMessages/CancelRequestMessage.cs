using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.RequestMessages
{
     public class CancelRequestMessage : EnhancedRequestMessage
     {
        
        public byte DH1;
        public byte RSV1;
        public byte[]? DH3;
        public byte RSV2;

        public CancelRequestMessage()
        {
            DH1 = 0x10;
            RSV1 = 0x00;
            DH3 = [0x00, 0x01];
            RSV2 = 0x00;
        }

        public override byte[] GetMessageBytes()
        {
            byte[] message =
            [
                DH0,
                DH1,
                DH2,
                RSV1,
                DH3[0],
                DH3[1],
                RSV2,
                FS,
            ];

            return message;
        }
    }
}
