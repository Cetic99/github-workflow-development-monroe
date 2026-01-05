using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;

public class CassetteBillsRejectedEvent : TransactionEvent
{
    public CassetteBillsRejectedEvent(DispenserCassetteBillCountStatus cassette, int billCount, DispenserBillCountStatus oldStatus, DispenserBillCountStatus newStatus) :
     base($"Rejected {billCount} {cassette.Currency.Symbol} bill{(billCount > 1 ? "s" : "")} from cassette #{cassette.CassetteNumber}")
    {
        RejectBinBillCount = billCount;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
    public DispenserCassetteBillCountStatus Cassette { get; private set; }
    public DispenserBillCountStatus OldStatus { get; private set; }
    public DispenserBillCountStatus NewStatus { get; private set; }
    public int RejectBinBillCount { get; private set; }
}
