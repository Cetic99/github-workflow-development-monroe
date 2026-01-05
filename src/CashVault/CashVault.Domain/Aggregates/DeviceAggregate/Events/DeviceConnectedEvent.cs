using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class DeviceConnectedEvent : DeviceEvent
    {
        public Device Device { get; private set; }
        public DeviceConnectedEvent(Device device, DeviceType deviceType)
            : base($"Device {device} connected", deviceType)
        {
            Device = device;
        }
    }
}
