using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.ResponseMessages
{
    internal class TemperatureResponse : DoorSensorResponse
    {
        private int SENSOR_ID; // for future use. Not used in this version
        public TemperatureResponse(byte[] data) : base(data)
        {
            MSG_LENGTH = data[1];
            CMD = data[2];

            if (CMD != 0x74)
            {
                throw new ArgumentException("Not temperature response msg!");
            }

            SENSOR_ID = data[3]; // for future use. Not used in this version

            bool temperatureNegative = (data[4] == 0x01);

            Temperature = data[5] + (data[6] / 100.0f); // Convert to float
            if (temperatureNegative)
            {
                // If the temperature is negative, convert it to negative float
                Temperature = -Temperature;
            }
        }

        public float Temperature { get; private set; }
    }
}
