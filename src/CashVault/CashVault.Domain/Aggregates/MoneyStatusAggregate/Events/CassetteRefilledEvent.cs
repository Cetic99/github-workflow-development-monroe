using CashVault.Domain.Common.Events;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events
{
    public class CassetteRefilledEvent : TransactionEvent
    {
        public CassetteRefilledEvent(DispenserCassetteBillCountStatus cassette, int billCount, DispenserBillCountStatus oldStatus, DispenserBillCountStatus newStatus)
            : base($"Cassette #{cassette.CassetteNumber} refilled with: {billCount * cassette.BillDenomination} {Currency.Default.Symbol}")
        {
            Cassette = cassette;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            Amount = billCount * cassette.BillDenomination;
        }

        public DispenserCassetteBillCountStatus Cassette { get; private set; }
        public DispenserBillCountStatus OldStatus { get; private set; }
        public DispenserBillCountStatus NewStatus { get; private set; }
        public decimal Amount { get; private set; }
    }
}
