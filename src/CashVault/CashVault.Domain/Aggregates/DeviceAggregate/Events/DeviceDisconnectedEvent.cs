using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class DeviceDisconnectedEvent : DeviceEvent
    {
        public Device Device { get; private set; }
        public DeviceDisconnectedEvent(Device device, DeviceType deviceType)
            : base($"Device {device} disconnected", deviceType)
        {
            Device = device;
        }
    }
}
