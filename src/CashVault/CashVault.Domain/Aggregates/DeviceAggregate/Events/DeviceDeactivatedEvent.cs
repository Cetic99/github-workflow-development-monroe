using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class DeviceDeactivatedEvent : DeviceEvent
    {
        public Device Device { get; private set; }
        public DeviceDeactivatedEvent(Device device, DeviceType deviceType)
            : base($"Device {device} activated", deviceType)
        {
            Device = device;
        }
    }
}
