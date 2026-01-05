using CashVault.CabinetControlUnitDriver.Interfaces;
using CashVault.CabinetControlUnitDriver.Messages.Common;
using CashVault.DeviceDriver.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.ResponseMessages
{
    internal abstract class DoorSensorResponse : ICabinetControlUnit
    {
        public byte[] data;
        public byte HEADER { get; init; }
        public byte MSG_LENGTH { get; init; }
        public byte CMD { get; init; }
        public byte[]? PAYLOAD { get; init; }
        public byte[]? CRC { get; init; }


        public DoorSensorResponse(byte[] data)
        {
            this.data = data;

            if (data == null)
            {
                throw new ArgumentException("No data!");
            }

            if (data[0] != 0x22)
            {
                throw new ArgumentException("Not response msq!");
            }
            HEADER = data[0];
        }
        public DateTime TimeStamp
        {
            get { return DateTime.UtcNow; }
        }

        public byte[] GetMessageBytes()
        {
            return data;
        }

        bool ICabinetControlUnit.IsValidMessage()
        {
            return true;
        }
    }
}

