using System;

namespace CashVault.Domain.Common.Exceptions
{
    public class BaseException : Exception
    {

        public BaseException(string message, Exception ex) : base(ex.Message, ex)
        {
        }

        public BaseException(string message) : base(message)
        {
        }
    }
}
