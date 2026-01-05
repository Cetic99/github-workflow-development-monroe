using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class DeviceDisabledEvent : DeviceEvent
    {
        public DeviceDisabledEvent(Device device, DeviceType deviceType)
            : base($"Device {device} disabled", deviceType)
        {
            Device = device;
        }

        public Device Device { get; }
    }
}
