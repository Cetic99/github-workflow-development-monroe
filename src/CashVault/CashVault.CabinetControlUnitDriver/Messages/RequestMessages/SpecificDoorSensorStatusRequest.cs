using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.RequestMessages
{
    public class SpecificDoorSensorStatusRequest : BaseCommand
    {
        public byte CMD;
        public byte PAYLOAD;

        public SpecificDoorSensorStatusRequest(int sensorId)
        {
            CMD = 0x73;
            PAYLOAD = (byte)sensorId;
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
