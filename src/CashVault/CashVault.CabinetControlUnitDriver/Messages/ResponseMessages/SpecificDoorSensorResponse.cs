using CashVault.CabinetControlUnitDriver.Messages.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CashVault.CabinetControlUnitDriver.Messages.ResponseMessages
{
    internal class SpecificDoorSensorResponse : DoorSensorResponse
    {
        public DoorSensorStatus doorStatus { get; set; } = new();
        public SpecificDoorSensorResponse(byte[] data) : base(data)
        {

            if (data[2] != 0x73)
            {
                throw new ArgumentException("Not specific door sensors response msg!");
            }
            CMD = data[2];
            MSG_LENGTH = data[1];
            ParsePayload(data.Skip(3).Take(MSG_LENGTH - 5).ToArray());
            CRC = data.Skip(MSG_LENGTH - 2).Take(2).ToArray();
        }

        private void ParsePayload(byte[] payload)
        {
            if (payload[1] == 0x80)
            {
                doorStatus = new DoorSensorStatus
                {
                    SensorId = Convert.ToInt32(payload[0]),
                    Opened = false
                };
            }
            else if (payload[1] == 0x8F)
            {
                doorStatus = new DoorSensorStatus
                {
                    SensorId = Convert.ToInt32(payload[0]),
                    Opened = true
                };
            }
            else
            {
                throw new ArgumentException("Unknown argument!\n");
            }
        }
    }
}
