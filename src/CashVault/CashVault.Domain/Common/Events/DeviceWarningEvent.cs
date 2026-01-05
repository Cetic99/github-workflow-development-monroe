using CashVault.Domain.Aggregates.DeviceAggregate;

namespace CashVault.Domain.Common.Events
{
    public class DeviceWarningEvent : DeviceEvent
    {
        protected DeviceWarningEvent(string message) : base(message) { }
        public DeviceWarningEvent(string message, DeviceType deviceType) : base(message, deviceType) { }
    }
}
