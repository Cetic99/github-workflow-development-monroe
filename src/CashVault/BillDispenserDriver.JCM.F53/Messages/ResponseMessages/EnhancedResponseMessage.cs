using CashVault.BillDispenserDriver.JCM.F53.Interfaces;
using CashVault.BillDispenserDriver.JCM.F53.Messages.Common;

namespace CashVault.BillDispenserDriver.JCM.F53.Messages.ResponseMessages
{
    internal abstract class EnhancedResponseMessage : IBillDispenserResponse
    {
        private byte[] data;

        private bool isPositive = false;

        /// <summary>
        /// The first byte of the response message, which determines will message be positive or negative
        /// </summary>
        protected byte DH0 { get; init; }

        /// <summary>
        /// The third byte of the response message, which determines the type of the response message
        /// </summary>
        protected byte DH1 { get; init; }

        /// <summary>
        /// The second byte of the response message, which is always 0xFF
        /// </summary>
        protected byte DH2 { get; init; }

        /// <summary>
        /// Reserved for future use, always 0x00
        /// </summary>
        protected byte RSV { get; init; }

        /// <summary>
        /// Command specific data, 2 bytes
        /// </summary>
        protected byte[] DH3 { get; init; }

        /// <summary>
        /// Part of the response message that is common to all messages and contains all the information about the response
        /// </summary>
        public ResponseCommonPart CommonPart { get; init; }

        /// <summary>
        /// Message separator, always 0x1C
        /// </summary>
        protected byte FS { get; init; }



        public EnhancedResponseMessage(byte[] data)
        {
            this.data = data;

            if (data.Length < 90)
            {
                throw new ArgumentException("Data length is less than 90 bytes");
            }

            // DH0 check
            if (data[0] != 0xF0 && data[0] != 0xE0)
            {
                throw new ArgumentException("Data does not start with 0xF0 or 0xE0");
            }

            DH0 = data[0];
            isPositive = (DH0 == 0xE0);

            // DH1 check should be performed in the derived class

            // DH2 check
            if (data[2] != 0xFF)
            {
                throw new ArgumentException("Data does not contain 0xFF at position 2");
            }
            DH2 = data[2];

            // RSV check
            if (data[3] != 0x00)
            {
                throw new ArgumentException("Data does not contain 0x00 at position 3");
            }
            RSV = data[3];

            // DH3 check should be performed in the derived class

            CommonPart = new ResponseCommonPart(data.Skip(6).Take(84).ToArray());

            // FS check
            if (data[data.Length - 1] != 0x1C)
            {
                throw new ArgumentException("Data does not end with 0x1C");
            }
            FS = data[data.Length - 1];
        }

        public bool IsEnhanced()
        {
            return true;
        }

        public byte[] GetMessageBytes()
        {
            return data;
        }

        public bool IsPositive()
        {
            return isPositive;
        }

        public virtual bool IsValidMessage()
        {
            throw new NotImplementedException();
        }

        public DateTime TimeStamp
        {
            get { return DateTime.UtcNow; }
        }

        public override string ToString()
        {
            return BitConverter.ToString(GetMessageBytes()).Replace("-", "");
        }

    }
}
