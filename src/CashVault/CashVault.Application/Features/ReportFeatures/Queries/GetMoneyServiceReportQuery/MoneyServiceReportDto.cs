using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.DeviceFeatures.Queries;

public class MoneyServiceReportDto
{
    public int AcceptorBillCount { get; set; }
    public decimal AcceptorBillAmount { get; set; }
    public int AcceptorTicketCount { get; set; }
    public decimal AcceptorTicketAmount { get; set; }
    public int AcceptorCoinCount { get; set; }
    public decimal AcceptorCoinAmount { get; set; }
    public int AcceptorRejectedBillsCount { get; set; }
    public int AcceptorRejectedTicketsCount { get; set; }
    public int DispenserRejectedBillsCount { get; set; }

    public List<BillDispenserCassette> DispenserCassettes { get; set; }

    public decimal TotalAmount => AcceptorBillAmount + DispenserCassettes.Sum(c => c.TotalAmount) + AcceptorCoinAmount;
    public decimal DispenserTotalAmount => DispenserCassettes?.Sum(c => c.TotalAmount) ?? 0.0m;
    public decimal AcceptorTotalAmount => AcceptorBillAmount + AcceptorCoinAmount;
    public string CurrencySymbol => Currency.Default.Symbol;

    public MoneyServiceReportDto()
    {
        AcceptorBillCount = 0;
        AcceptorBillAmount = 0;
        AcceptorTicketCount = 0;
        AcceptorTicketAmount = 0;
        AcceptorCoinCount = 0;
        AcceptorCoinAmount = 0;
        AcceptorRejectedBillsCount = 0;
        AcceptorRejectedTicketsCount = 0;
        DispenserRejectedBillsCount = 0;
        DispenserCassettes = [];
    }
}

public class BillDispenserCassette
{
    public string Name { get; set; } = string.Empty;
    public int BillDenomination { get; set; }
    public int BillCount { get; set; }
    public int TotalAmount => BillDenomination * BillCount;
    public int CassetteNumber { get; set; }
}