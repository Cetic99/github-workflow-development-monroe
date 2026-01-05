namespace CashVault.Application.Features.LogsFeatures.Queries;

public class AcceptorItemDto
{
    public int OldBillCount { get; set; }
    public int NewBillCount { get; set; }
    public decimal OldBillAmount { get; set; }
    public decimal NewBillAmount { get; set; }
    public int OldTicketCount { get; set; }
    public int NewTicketCount { get; set; }
    public decimal OldTicketAmount { get; set; }
    public decimal NewTicketAmount { get; set; }
}
