using Microsoft.Extensions.Logging;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class ServerConfiguration
{
    public string ServerUrl { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = false;
    public int MinimalLogLevel { get; set; } = (int)LogLevel.Information;
    public int SendInterval { get; set; } = 3;

    public ServerConfiguration()
    {
        MinimalLogLevel = (int)LogLevel.Information;
        SendInterval = 3;
    }
}