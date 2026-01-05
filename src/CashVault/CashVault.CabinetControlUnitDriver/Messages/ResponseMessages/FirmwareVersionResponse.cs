using CashVault.CabinetControlUnitDriver.Messages.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.ResponseMessages
{
    internal class FirmwareVersionResponse : DoorSensorResponse
    {
        public FirmwareVersionResponse(byte[] data) : base(data)
        {
            if (data[2] != 0x66)
            {
                throw new ArgumentException("Not FW version response msg!");
            }

            CMD = data[2];
            MSG_LENGTH = data[1];
            PAYLOAD = data.Skip(3).Take(MSG_LENGTH - 5).ToArray();
            CRC = data.Skip(MSG_LENGTH - 2).Take(2).ToArray();
        }
    }
}
