namespace CashVault.Infrastructure.Server.Dtos;

internal class DeviceStatus
{
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Status { get; set; } = null!;
    public bool IsEnabled { get; set; } = false;
    public bool IsActive { get; set; } = false;
    public bool IsConnected { get; set; } = false;
    public string? Warning { get; set; }
    public string? Error { get; set; }
    public string? AdditionalInformation { get; set; }
}