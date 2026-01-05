namespace CMSMock.Dtos;

public class TicketRedemptionResponseModel
{
    public bool IsSuccessful { get; set; }

    public string Type { get; set; } = null!;
    public string DateTime { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string MachineName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public int ResponseCode { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountWithTaxes { get; set; }
    public decimal Taxes { get; set; }
    public string? Reason { get; set; }
}
