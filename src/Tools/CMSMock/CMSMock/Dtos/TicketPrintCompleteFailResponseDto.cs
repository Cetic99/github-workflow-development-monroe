namespace CMSMock.Dtos;

internal class TicketPrintCompleteFailResponseDto
{
    public string Type { get; set; } = null!;
    public DateTime DateTime { get; set; }
    public string Language { get; set; } = null!;
    public string MachineName { get; set; } = null!;
    public string Barcode { get; set; }
    public int ResponseCode { get; set; }
}
