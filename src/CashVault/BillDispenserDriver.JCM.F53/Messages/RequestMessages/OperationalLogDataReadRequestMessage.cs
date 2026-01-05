using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Note: Implement handle for P1 and P2 parameters!
 * P1: Data type
        0x00: Measured information
        0x01: Operational log
        0x02: Command/ Response log
        0x03: Thickness information
        0xFF: All information deletion
 * P2: Block No.
        0x00～0x07: Measured information
        0x00～0x07: Operational log
        0x00～0x03: Command/ Response log
        0x00～0x01: Thickness information
        0xFF: Information deletion specified with P1.
 */

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.RequestMessages
{
    public class OperationalLogDataReadRequestMessage : EnhancedRequestMessage
    {
        public byte DH1;
        public byte RSV;
        public byte[]? DH3;
        public byte P1;
        public byte P2;

        public OperationalLogDataReadRequestMessage(byte p1, byte p2)
        {
            DH1 = 0x13;
            RSV = 0x00;
            DH3 = [0x00, 0x02];
            P1 = p1;
            P2 = p2;
            
        }

        public override byte[] GetMessageBytes()
        {
            byte[] message =
            [
                DH0,
                DH1,
                DH2,
                RSV,
                DH3[0],
                DH3[1],
                P1,
                P2,
                FS,
            ];

            return message;
        }
    }
}
