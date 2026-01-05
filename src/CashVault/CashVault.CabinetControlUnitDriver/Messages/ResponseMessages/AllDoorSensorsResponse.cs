using CashVault.CabinetControlUnitDriver.Messages.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.ResponseMessages
{
    internal class AllDoorSensorsResponse : DoorSensorResponse
    {
        public List<DoorSensorStatus>? SensorStatus { get; set; } = new();
        public AllDoorSensorsResponse(byte[] data) : base(data)
        {

            if (data[2] != 0x53)
            {
                throw new ArgumentException("Not all door sensors response msg!");
            }
            CMD = data[2];
            MSG_LENGTH = data[1];
            CRC = data.Skip(MSG_LENGTH - 2).Take(2).ToArray();
            ParsePayload(data.Skip(3).Take(MSG_LENGTH - 5).ToArray());
        }

        private void ParsePayload(byte[] payload)
        {
            for (int i = 0; i < MSG_LENGTH - 5; i++)
            {
                if (payload[i] == 0x80)
                {
                    SensorStatus.Add(new DoorSensorStatus
                    {
                        SensorId = i + 1,
                        Opened = false,
                    });
                }
                else if (payload[i] == 0x8F)
                {
                    SensorStatus.Add(new DoorSensorStatus
                    {
                        SensorId = i + 1,
                        Opened = true,
                    });
                }
                else
                {
                    throw new ArgumentException("Unknown argument!\n");
                }
            }
        }

    }
}
