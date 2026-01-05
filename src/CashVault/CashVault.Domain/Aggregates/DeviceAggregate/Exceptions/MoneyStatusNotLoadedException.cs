using System;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Exceptions
{
    public class MoneyStatusNotLoadedException : Exception
    {
        public MoneyStatusNotLoadedException(string message)
            : base(message) { }

        public static void ThrowIfNotLoaded(bool isLoaded, string status)
        {
            if ((!isLoaded))
            {
                Throw($"MoneStatus.{status} not loaded from configuration");
            }
        }

        internal static void Throw(string message) =>
            throw new MoneyStatusNotLoadedException(message);
    }
}
