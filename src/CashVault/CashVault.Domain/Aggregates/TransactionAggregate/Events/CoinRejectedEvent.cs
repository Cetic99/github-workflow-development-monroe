using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.TransactionAggregate.Events
{
    public class CoinRejectedEvent : DeviceFailEvent
    {
        public CoinRejectedEvent(string message)
            : base(string.IsNullOrEmpty(message) ? "Coin rejected" : message)
        {
        }
    }
}
