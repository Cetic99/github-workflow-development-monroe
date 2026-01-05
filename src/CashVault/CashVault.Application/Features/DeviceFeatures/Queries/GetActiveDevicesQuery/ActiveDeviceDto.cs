namespace CashVault.Application.Features.DeviceFeatures.Queries.GetActiveDevicesQuery
{
    public record ActiveDeviceDto
    {
        public string Type { get; init; } = null!;
        public string Name { get; init; } = null!;
        public string Status { get; init; } = null!;
        public bool IsEnabled { get; init; } = false;
        public bool IsActive { get; init; } = false;
        public bool IsConnected { get; init; } = false;
    }
}
