using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.RequestMessages
{
    public class SensorTransitionDetectionRequestMessage : EnhancedRequestMessage
    {
        private static int sensorMaskLength = 9;
        private static int messageLength = 17;
        public byte DH1;
        public byte[] DH3;
        public byte RSV1;
        public byte[] sensorMask = new byte[sensorMaskLength];
        public byte RSV2;
        private byte[] message = new byte[messageLength];

        public SensorTransitionDetectionRequestMessage()
        {
            DH1 = 0x14;
            RSV1 = 0x00;
            DH3 = [0x00, 0x0A];
            RSV2 = 0x00;
            sensorMask = [0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x00, 0x00, 0x1F];

        }

        public override byte[] GetMessageBytes()
        {
            byte[] msg1 =
            [
                DH0,
                DH1,
                DH2,
                RSV1,
                DH3[0],
                DH3[1],
            ];

            byte[] msg2 =
            [
                RSV2,
                FS,
            ];

            Array.Copy(msg1, message, msg1.Length);
            Array.Copy(sensorMask, 0, message, msg1.Length, sensorMask.Length);
            Array.Copy(msg2, 0, message, msg1.Length + sensorMask.Length, msg2.Length);

            return message;
        }
    }
}
