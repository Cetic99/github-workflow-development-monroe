using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.ResponseMessages
{
    internal class InvalidCommandResponse : DoorSensorResponse
    {
        public InvalidCommandResponse(byte[] data) : base(data)
        {
            if (data[2] != 0x45)
            {
                throw new ArgumentException("Not invalid command response msg!");
            }

            CMD = data[2];
            MSG_LENGTH = data[1];
            PAYLOAD = [data[3], 0x00]; //because all other payloads are array
            CRC = data.Skip(MSG_LENGTH - 2).Take(2).ToArray();
        }
    }
}
