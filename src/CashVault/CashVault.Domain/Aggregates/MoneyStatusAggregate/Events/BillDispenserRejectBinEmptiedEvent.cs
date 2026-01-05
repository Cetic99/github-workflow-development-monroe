using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;

public class BillDispenserRejectBinEmptiedEvent : TransactionEvent
{
    public BillDispenserRejectBinEmptiedEvent(int billCount, DispenserBillCountStatus oldStatus, DispenserBillCountStatus newStatus) :
        base($"Bill Dispenser Reject Bin Emptied with bill count {billCount}")
    {
        RejectBinBillCount = billCount;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }

    public DispenserBillCountStatus OldStatus { get; private set; }
    public DispenserBillCountStatus NewStatus { get; private set; }
    public int RejectBinBillCount { get; private set; }
}
