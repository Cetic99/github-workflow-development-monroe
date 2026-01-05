using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class BillDispenserConfiguredEvent : DeviceEvent
    {
        public BillDispenserConfiguredEvent(IBillDispenserConfiguration billDispenserConfiguration)
            : base("Bill dispenser configured", DeviceAggregate.DeviceType.BillDispenser)
        {
            BillDispenserConfiguration = billDispenserConfiguration;
        }

        public IBillDispenserConfiguration BillDispenserConfiguration { get; private set; }
    }
}
