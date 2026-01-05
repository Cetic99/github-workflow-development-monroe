namespace CashVault.Application.Features.DeviceFeatures.Queries;

public record NetworkAdapterDto
{
    public bool IsDhcpEnabled { get; set; }
    public bool IsDnsEnabled { get; set; }
    public bool HasAdminPrivilages { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public NetworkAdapterInfoDto? NetworkAdapterInfo { get; set; }
}


public record NetworkAdapterInfoDto
{
    public string? IpAddress { get; set; } = string.Empty;
    public string? NetworkMask { get; set; } = string.Empty;
    public string? Gateway { get; set; } = string.Empty;
    public string? PreferredDns { get; set; } = string.Empty;
    public string? AlternateDns { get; set; } = string.Empty;
}