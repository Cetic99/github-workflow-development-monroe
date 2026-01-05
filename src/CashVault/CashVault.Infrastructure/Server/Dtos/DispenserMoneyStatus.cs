namespace CashVault.Infrastructure.Server.Dtos;

internal class DispenserMoneyStatus
{
    public List<CassetteStatus> Cassettes { get; set; } = [];
}
