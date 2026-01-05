using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class DeviceEnabledEvent : DeviceEvent
    {
        public Device Device { get; private set; }
        public DeviceEnabledEvent(Device device, DeviceType deviceType) : base($"Device {device} enabled", deviceType)
        {
            Device = device;
        }
    }
}
