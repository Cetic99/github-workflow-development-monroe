using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Events
{
    public class CassettesUpdatedFromDispnserConfigEvent : DeviceEvent
    {
        public CassettesUpdatedFromDispnserConfigEvent(string? message = "Cassettes updated from dispenser configuration")
            : base(message, DeviceAggregate.DeviceType.BillDispenser) { }
    }
}
