using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;

public class CassetteBillCountDecreasedEvent : TransactionEvent
{
    public CassetteBillCountDecreasedEvent(DispenserCassetteBillCountStatus cassette, int billCount, DispenserBillCountStatus oldStatus, DispenserBillCountStatus newStatus)
    : base($"Cassette ${cassette.CassetteNumber} decreased for {cassette.BillDenomination * billCount}")
    {
        Cassette = cassette;
        Amount = cassette.BillDenomination * billCount;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }

    public DispenserCassetteBillCountStatus Cassette { get; private set; }
    public DispenserBillCountStatus OldStatus { get; private set; }
    public DispenserBillCountStatus NewStatus { get; private set; }
    public decimal Amount { get; private set; }
}
