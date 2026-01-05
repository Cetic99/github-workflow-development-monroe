namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class OnlineIntegrationsConfiguration
{
    public bool CasinoManagementSystem { get; set; }
    public string? Url { get; set; }
    public string? DeviceId { get; set; }
    public string? SecretKey { get; set; }
    public int TimeoutInSeconds { get; set; }
}
