using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events;

public class CabinetDoorOpenedEvent : DeviceEvent
{
    public int DoorNumber { get; private set; }

    public CabinetDoorOpenedEvent(int doorNumber)
        : base($"Cabinet door {doorNumber} opened", DeviceAggregate.DeviceType.Cabinet)
    {
        DoorNumber = doorNumber;
    }
}
