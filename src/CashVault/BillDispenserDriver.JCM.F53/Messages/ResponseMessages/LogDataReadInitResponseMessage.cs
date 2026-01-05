using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.ResponseMessages
{
    internal class LogDataReadInitResponseMessage : EnhancedResponseMessage
    {
        public byte DH1;
        public byte[] DH3;
        public byte OPR;
        public byte[] messageData = new byte[40];
        public byte[] totalCountersCassette1 = new byte[32];
        public byte[] totalCountersCassette2 = new byte[32];
        public byte[] totalCountersCassette3 = new byte[32];
        public byte[] totalCountersCassette4 = new byte[32];
        public byte[] totalCountersCassette5 = new byte[32];
        public byte[] totalCountersCassette6 = new byte[32];
        public byte[] totalCountersCassette7 = new byte[32];
        public byte[] totalCountersCassette8 = new byte[32];

        public LogDataReadInitResponseMessage(byte[] data) : base(data)
        {
            throw new NotImplementedException();
        }

        //BillDispenserLogDataReadInitResponseMessage()
        //{
        //    DH1 = 0x12;
        //    DH3 = [0x01, 0x7D];
        //}
    }
}
