using Microsoft.AspNetCore.SignalR;

namespace CashVault.WebAPI.Hubs;

public class HeartbeatHub : Hub<IHeartbeatHub>
{
    private ILogger<HeartbeatHub> _logger;

    public HeartbeatHub(ILogger<HeartbeatHub> logger)
    {
        _logger = logger;
    }

    public async Task Heartbeat(string payload)
    {
        await Clients.All.Heartbeat(payload);
    }

    public async Task CMSConnectivityStatus(string status)
    {
        await Clients.All.CMSConnectivityStatus(status);
    }
}

public interface IHeartbeatHub
{
    Task Heartbeat(string payload);
    Task CMSConnectivityStatus(string status);
    Task MessageUpdated(string payload);
    Task UserWidgetsUpdated(string payload);
}
