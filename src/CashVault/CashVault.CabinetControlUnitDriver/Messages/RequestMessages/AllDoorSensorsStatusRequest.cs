using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.RequestMessages
{
    public class AllDoorSensorsStatusRequest : BaseCommand
    {
        public byte CMD;
        public byte PAYLOAD;

        public AllDoorSensorsStatusRequest()
        {
            CMD = 0x53;
            PAYLOAD = 0x61;
        }

        public override byte[] GetMessageBytes()
        {
            byte[] message =
            [
                HEADER,
                MSG_LENGTH,
                CMD,
                PAYLOAD,
                CRC[0],
                CRC[1],
            ];

            return message;
        }
    }
}
