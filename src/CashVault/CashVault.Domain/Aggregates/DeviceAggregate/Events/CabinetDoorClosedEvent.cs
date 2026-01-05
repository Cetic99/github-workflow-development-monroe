using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class CabinetDoorClosedEvent : DeviceEvent
{
    public int DoorNumber { get; private set; }

    public CabinetDoorClosedEvent(int doorNumber)
        : base($"Cabinet door {doorNumber} closed", DeviceAggregate.DeviceType.Cabinet)
    {
        DoorNumber = doorNumber;
    }
}
