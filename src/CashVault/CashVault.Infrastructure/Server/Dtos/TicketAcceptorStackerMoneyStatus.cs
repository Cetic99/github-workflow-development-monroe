namespace CashVault.Infrastructure.Server.Dtos;

internal class TicketAcceptorStackerMoneyStatus
{
    public int BillCount { get; set; }
    public decimal BillAmount { get; set; }
    public int TicketCount { get; set; }
    public decimal TicketAmount { get; set; }
}
