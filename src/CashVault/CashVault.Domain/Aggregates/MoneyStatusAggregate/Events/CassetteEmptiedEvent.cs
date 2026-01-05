using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events
{
    public class CassetteEmptiedEvent : TransactionEvent
    {
        public CassetteEmptiedEvent(DispenserCassetteBillCountStatus cassette, int oldBillCount, DispenserBillCountStatus oldStatus, DispenserBillCountStatus newStatus)
            : base($"Cassette #{cassette.CassetteNumber} emptied")
        {
            Cassette = cassette;
            Amount = cassette.BillDenomination * oldBillCount;
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }

        public DispenserCassetteBillCountStatus Cassette { get; private set; }
        public DispenserBillCountStatus OldStatus { get; private set; }
        public DispenserBillCountStatus NewStatus { get; private set; }
        public decimal Amount { get; private set; }
    }
}
