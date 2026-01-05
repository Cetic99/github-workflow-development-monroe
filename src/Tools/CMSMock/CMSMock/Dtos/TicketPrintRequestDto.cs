namespace CMSMock.Dtos;

internal class TicketPrintRequestDto
{
    public string Type { get; set; } = null!;
    public DateTime DateTime { get; set; }
    public string Language { get; set; } = null!;
    public string MachineName { get; set; } = null!;
    public decimal Amount { get; set; }
}
