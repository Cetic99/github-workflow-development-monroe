namespace CashVault.Infrastructure.Server.Dtos;

internal class MoneyStatus
{
    public TicketAcceptorStackerMoneyStatus Acceptor { get; set; } = null!;
    public DispenserMoneyStatus Dispenser { get; set; } = null!;
    public decimal TotalAmount { get; set; } = 0;
}
