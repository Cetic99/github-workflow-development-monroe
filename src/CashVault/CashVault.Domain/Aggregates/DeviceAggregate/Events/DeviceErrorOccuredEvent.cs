using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class DeviceErrorOccuredEvent : DeviceFailEvent
    {
        public DeviceErrorOccuredEvent(DeviceType deviceType, string errorMessage)
            : base(errorMessage, deviceType)
        {
        }
    }
}
