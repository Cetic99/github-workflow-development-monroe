using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events
{
    public class BillRejectedEvent : DeviceFailEvent
    {
        public BillRejectedEvent(string message)
            : base(string.IsNullOrEmpty(message) ? "Bill rejected" : message, DeviceAggregate.DeviceType.BillAcceptor)
        {
        }
    }
}
