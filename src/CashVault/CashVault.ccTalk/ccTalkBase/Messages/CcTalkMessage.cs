using CashVault.DeviceDriver.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.ccTalk.ccTalkBase.Messages
{
    /// <summary>
    ///  Incapsulates fields of generic cctalk message (command or respond). Implemets serializatoin.
    /// </summary>
    public class CctalkMessage : ISerialPortMessage
    {
        /// <summary>
        /// Minimal possible message length
        /// </summary>
        public static readonly byte MinMessageLength = 5;

        /// <summary>
        ///  Maximal posible data length
        /// </summary>
        public static readonly byte MaxDataLength = 252;

        public static readonly byte PosDestAddr = 0;
        public static readonly byte PosDataLen = 1;
        public static readonly byte PosSourceAddr = 2;
        public static readonly byte PosHeader = 3;
        public static readonly byte PosDataStart = 4;

        /// <summary>
        ///  Destination device cctalk address. 0 - broadcast (single device on bus only).
        /// </summary>
        public byte DestAddr { set; get; }
        /// <summary>
        /// Source device cctalk address. 1 for host devices. When CRC checksum used - there is second byte for checksum.
        /// </summary>
        public byte SourceAddr { set; get; }

        /// <summary>
        ///  Message header. Command or respond code.
        /// </summary>
        public byte Header { get; set; }

        /// <summary>
        ///  Dete for message. Format depends on header.
        /// </summary>
        public byte[] Data;

        /// <summary>
        ///  Length of data in bytes
        /// </summary>
        public byte DataLength
        {
            get { return (byte)(Data == null ? 0 : Data.Length); }
        }

        public DateTime TimeStamp
        {
            get { return DateTime.UtcNow; }
        }

        /// <summary>
        ///  Serializes message for transfer, but does not apply checksum
        /// </summary>
        public byte[] GetTransferDataNoChecksumm()
        {
            byte[] msgData = Data;
            var msgDataLen = (byte)(msgData == null ? 0 : msgData.Length);

            if (msgDataLen > MaxDataLength)
                throw new InvalidOperationException("Data too long. " + GetType().Name);

            var msg = new byte[MinMessageLength + msgDataLen];
            msg[PosDestAddr] = DestAddr;
            msg[PosDataLen] = msgDataLen;
            msg[PosSourceAddr] = SourceAddr;
            msg[PosHeader] = Header;

            if (msgData != null && msgDataLen > 0)
                Array.Copy(msgData, 0, msg, PosDataStart, msgData.Length);

            return msg;
        }

        public byte[] GetMessageBytes()
        {
            return Data;
        }

        //public CctalkMessage(byte[] rawData)
        //{
        //    Data = rawData;

        //    if (rawData != null && rawData.Length >= MinMessageLength)
        //    {
        //        DestAddr = rawData[PosDestAddr];
        //        SourceAddr = rawData[PosSourceAddr];
        //        Header = rawData[PosHeader];
        //    }
        //}
    }
}
