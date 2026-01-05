using CashVault.Domain.Aggregates.DeviceAggregate;

namespace CashVault.Domain.Common.Events
{
    public class DeviceFailEvent : DeviceEvent
    {
        protected DeviceFailEvent(string message) : base(message) { }

        public DeviceFailEvent(string message, DeviceType deviceType) : base(message, deviceType)
        {
        }
    }
}
