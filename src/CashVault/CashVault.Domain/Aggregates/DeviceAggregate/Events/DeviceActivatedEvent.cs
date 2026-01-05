using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class DeviceActivatedEvent : DeviceEvent
    {
        public Device Device { get; private set; }
        public DeviceActivatedEvent(Device device, DeviceType deviceType)
            : base($"Device {device} activated", deviceType)
        {
            Device = device;
        }
    }
}
