namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class LogItemDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Message { get; set; }
        public string Type { get; set; } = null!;
        public string? DeviceType { get; init; }
        public string? Name { get; set; }
    }
}