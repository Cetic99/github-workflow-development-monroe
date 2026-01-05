namespace CashVault.Infrastructure.Server.Dtos;

public class ServerActionResponseDto
{
    public Guid Uuid { get; set; }
    public string? ExternalId { get; set; }
    public bool IsSuccess { get; set; } = false;
    public string? Message { get; set; } = string.Empty;
}
