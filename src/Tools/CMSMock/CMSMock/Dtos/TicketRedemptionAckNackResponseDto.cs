namespace CMSMock.Dtos;

internal class TicketRedemptionAckNackResponseDto
{
    public string Type { get; set; } = null!;
    public DateTime DateTime { get; set; }
    public string MachineName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public int ResponseCode { get; set; }
}
