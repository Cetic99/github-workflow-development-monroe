using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.RequestMessages
{
    public class TemperatureRequest : BaseCommand
    {
        byte CMD;
        byte PAYLOAD;

        public TemperatureRequest()
        {
            CMD = 0x74; // Command for temperature request
            PAYLOAD = 0x00; // Payload for temperature request
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
