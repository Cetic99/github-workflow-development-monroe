using System;

namespace CashVault.ccTalk.Common.Exceptions
{
    public class DeviceCommunicationException : Exception
    {
        public DeviceCommunicationException(string message) : base(message)
        {
        }

        public DeviceCommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}