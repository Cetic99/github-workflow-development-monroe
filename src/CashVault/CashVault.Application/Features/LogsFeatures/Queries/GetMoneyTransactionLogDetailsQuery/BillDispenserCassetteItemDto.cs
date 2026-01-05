namespace CashVault.Application.Features.LogsFeatures.Queries;

public class BillDispenserCassetteItemDto
{
    public int CassetteNumber { get; set; }
    public int BillDenomination { get; set; }
    public int OldBillCount { get; set; }
    public int NewBillCount { get; set; }
}
