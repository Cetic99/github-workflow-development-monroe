namespace CMSMock.Dtos;

internal class TransactionRequestDto
{
    public string Type { get; set; } = null!;
    public DateTime DateTime { get; set; }
    public string MachineName { get; set; } = null!;
    public string TransactionCode { get; set; } = null!;
    public MoneyStatus MoneyStatus { get; set; } = null!;
}

internal class MoneyStatus
{
    public List<DispenserCassetteStatus> Cassettes { get; set; } = [];
}

internal class DispenserCassetteStatus
{
    public int CassetteNumber { get; set; }
    public string? CurrencyIsoCode { get; set; }
    public int BillDenomination { get; set; }
    public int CurrentBillCount { get; set; }
}

