using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.TransactionAggregate;

public class DispenserBillTransactionItem : Entity
{
    public int CassetteNumber { get; private set; }
    public int BillDenomination { get; private set; }
    public int BillCountRequested { get; private set; }
    public int BillCountRejected { get; private set; }
    public int BillCountDispensed { get; private set; }
    public int AmountDispensed => BillDenomination * BillCountDispensed;
    public int AmountRejected => BillDenomination * BillCountRejected;

    private DispenserBillTransactionItem() { }
    public DispenserBillTransactionItem(int cassetteNumber, int billDenomination, int billCountRequested)
    {
        CassetteNumber = cassetteNumber;
        BillDenomination = billDenomination;
        BillCountRequested = billCountRequested;
    }

    public void SetOutcome(int billCountDispensed, int billCountRejected)
    {
        BillCountDispensed = billCountDispensed;
        BillCountRejected = billCountRejected;
    }
}
