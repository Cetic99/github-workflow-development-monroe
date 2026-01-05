using CashVault.Domain.ValueObjects;

namespace CashVault.Infrastructure.Server.Dtos;

internal class CassetteStatus
{
    public int CassetteNumber { get; set; }
    public Currency Currency { get; set; }
    public int BillDenomination { get; set; }
    public int CurrentBillCount { get; set; }
}
