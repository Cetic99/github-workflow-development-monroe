using CashVault.DeviceDriver.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.CabinetControlUnitDriver.Messages.RequestMessages
{
    public class BaseCommand : ISerialPortMessage
    {
        //Header for all request messages
        public byte HEADER;
        public byte MSG_LENGTH;
        public byte[] CRC = new byte[2];

        public DateTime TimeStamp { get; private set; }

        public BaseCommand()
        {
            HEADER = 0x11;
            MSG_LENGTH = 0x06;
            TimeStamp = DateTime.UtcNow;
        }

        public virtual byte[] GetMessageBytes()
        {
            throw new NotImplementedException();
        }
    }
}
