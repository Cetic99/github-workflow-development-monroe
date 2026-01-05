namespace CMSMock.Dtos;

public class TicketPrintResponseModel
{
    public bool IsSuccessful { get; set; }

    public string Type { get; set; } = null!;
    public string DateTime { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string MachineName { get; set; } = null!;
    public int ResponseCode { get; set; }
    public string Barcode { get; set; } = null!;
    public decimal Amount { get; set; }
    public int Validity { get; set; }
    public string AmountText { get; set; } = string.Empty;
    public string AmountInWords { get; set; } = string.Empty;
    public DateTime DatePrint { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string? Address2 { get; set; }
}
