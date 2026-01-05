using System.Text.Json;
using CashVault.Application.Features.DeviceFeatures.Commands;
using CashVault.Application.Features.OperatorFeatures.Commands;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CashVault.WebAPI.Hubs;

internal class ReceiveMessagePayload
{
    public string messageType { get; set; } = string.Empty;
}

internal class NewIdentificationCardMessagePayload : ReceiveMessagePayload
{
    public int OperatorId { get; set; }
}

internal class OpenParcelLockerMessagePayload : ReceiveMessagePayload
{
    public string ParcelLocker { get; set; } = string.Empty;
    public string PostalService { get; set; } = string.Empty;
}

public class DeviceEventsHub : Hub<IDeviceEventsHub>
{
    private readonly ILogger<DeviceEventsHub> _logger;
    private readonly IMediator _mediator;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public DeviceEventsHub(ILogger<DeviceEventsHub> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task DeviceStatus(string key, string value)
    {
        await Clients.All.DeviceStatus(key, value);
    }

    public Task ReceiveError(string errorMessage)
    {
        _logger.LogError("Frontend error: {ErrorMessage}", errorMessage);
        return Task.CompletedTask;
    }

    public async Task ReceiveMessage(string area, string jsonMessage)
    {
        try
        {
            var payloadObject = JsonSerializer.Deserialize<ReceiveMessagePayload>(jsonMessage, _jsonSerializerOptions);
            string? messageType = payloadObject?.messageType;

            switch (messageType)
            {
                case "InitializeCardReader":
                    await SendCommand(new InitUserIdentificationCardReaderCommand());
                    break;

                case "ScanUserCard":
                    await SendCommand(new ScanUserCardCommand(), TimeSpan.FromMinutes(2));
                    break;

                case "CreateIdentificationCard":
                    var idCardPayload = JsonSerializer.Deserialize<NewIdentificationCardMessagePayload>(jsonMessage, _jsonSerializerOptions);

                    if (idCardPayload is not null)
                    {
                        await SendCommand(new AddOperatorIdentificationCardCommand()
                        {
                            OperatorId = idCardPayload.OperatorId

                        });
                    }

                    break;

                case "EnableUserLogin":
                    await SendCommand(new EnableUserLoginCommand());
                    break;

                case "DisableUserLogin":
                    await SendCommand(new DisableUserLoginCommand());
                    break;

                default:
                    await Clients.All.SendErrorMessage("Unknown message type received");
                    break;
            }
        }
        catch (Exception)
        {
            await Clients.All.SendErrorMessage("Global error");
        }
    }

    public async Task SendErrorMessage(string message)
    {
        var errorMessage = new { message };
        await Clients.All.SendMessage("ErrorMessage", JsonSerializer.Serialize(errorMessage));
    }

    #region Private Methods
    private async Task SendCommand(IBaseRequest command, TimeSpan? timespan = null)
    {
        using var cts = new CancellationTokenSource(timespan ?? TimeSpan.FromMinutes(10));

        try
        {
            var task = _mediator.Send(command, cts.Token);
            var completedTask = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cts.Token));
            if (completedTask == task)
            {
                await task;
            }
            else
            {
                await Clients.All.SendErrorMessage($"Timeout during user command: {nameof(command)}");
            }
        }
        catch (OperationCanceledException)
        {
            await Clients.All.SendErrorMessage($"Command: {nameof(command)} execution canceled due to timeout");
        }
        catch (Exception ex)
        {
            await Clients.All.SendErrorMessage($"Unhandled error: {ex.Message}, command: {nameof(command)}");
        }
    }
    #endregion
}

public interface IDeviceEventsHub
{
    Task DeviceStatus(string key, string value);
    Task ReceiveError(string errorMessage);
    Task ReceiveMessage(string area, string jsonMessage);
    Task SendMessage(string messageType, string payload);
    Task SendErrorMessage(string errorMessage);
}
