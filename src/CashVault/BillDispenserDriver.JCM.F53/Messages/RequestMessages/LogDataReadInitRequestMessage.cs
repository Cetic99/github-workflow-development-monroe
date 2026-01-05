using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

/*
 * Note: Implement handle for operation types
 * 
    OPR: Operation Type:
        00H: Read only
        01H: Clear error counter and retry counter
        02H: Clear pick counter
        03H: Clear all counters (error, retry, pick)
*/

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.RequestMessages
{
    public class LogDataReadInitRequestMessage : EnhancedRequestMessage
    {
        public static int msgLengthWithData = 48;
        public static int msgLengthWithoutData = 8;
        public static bool msgData;
        private byte[] message;

        public byte DH1;
        public byte RSV;
        public byte[] DH3;
        public byte OPR;
        public byte[] MSG = new byte[msgLengthWithData - msgLengthWithoutData];

        public LogDataReadInitRequestMessage(byte opr)
        {
            message = new byte[msgLengthWithoutData];
            msgData = false;
            DH1 = 0x12;
            RSV = 0x00;
            DH3 = [0x00, 0x01];
            OPR = opr;
        }
        public LogDataReadInitRequestMessage(byte opr, byte[] msg)
        {
            message = new byte[msgLengthWithData];
            msgData = true;
            DH1 = 0x12;
            RSV = 0x00;
            DH3 = [0x00, 0x29];
            OPR = opr;
            MSG = msg.ToArray();
        }

        public override byte[] GetMessageBytes()
        {
            if (msgData)
            {
                byte[] msg1 =
                [
                    DH0,
                    DH1,
                    DH2,
                    RSV,
                    DH3[0],
                    DH3[1],
                    OPR,
                ];

                Array.Copy(msg1, message, msg1.Length);
                Array.Copy(MSG, 0, message, msg1.Length, MSG.Length);
                message[msgLengthWithData - 1] = FS;
            }
            else
            {
                byte[] message =
                [
                    DH0,
                    DH1,
                    DH2,
                    RSV,
                    DH3[0],
                    DH3[1],
                    OPR,
                    FS,
                ];
            }

            return message;
        }
    }
}
