using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class DeviceWarningRaisedEvent : DeviceWarningEvent
    {
        public DeviceWarningRaisedEvent(DeviceType deviceType, string errorMessage)
            : base(errorMessage, deviceType)
        {
        }
    }
}
