using CashVault.Domain.Aggregates.DeviceAggregate;

namespace CashVault.Domain.Common.Events
{
    public class DeviceEvent : BaseEvent
    {

        public string? DeviceType { get; private set; }

        protected DeviceEvent(string message, DeviceType deviceType) : base(message)
        {
            DeviceType = deviceType.Code;
        }

        protected DeviceEvent(string message) : base(message) { }
    }
}
