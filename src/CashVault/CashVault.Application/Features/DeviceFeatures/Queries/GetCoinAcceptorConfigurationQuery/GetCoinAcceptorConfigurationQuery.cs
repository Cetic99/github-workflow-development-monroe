using System.Text.Json;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries;

public record GetCoinAcceptorConfigurationQuery : IRequest<JsonDocument?> { }

internal sealed class GetCoinAcceptorConfigurationQueryHandler : IRequestHandler<GetCoinAcceptorConfigurationQuery, JsonDocument?>
{
    private readonly ITerminalRepository _db;
    private readonly IDeviceDriverFactory _deviceDriverFactory;

    public GetCoinAcceptorConfigurationQueryHandler(ITerminalRepository db, IDeviceDriverFactory deviceDriverFactory)
    {
        _db = db;
        _deviceDriverFactory = deviceDriverFactory;
    }

    public async Task<JsonDocument?> Handle(GetCoinAcceptorConfigurationQuery req, CancellationToken cancellationToken)
    {
        var config = await _db.GetCoinAcceptorConfigurationAsync();
        return _deviceDriverFactory.ConvertConfigurationToJson(config);
    }
}
