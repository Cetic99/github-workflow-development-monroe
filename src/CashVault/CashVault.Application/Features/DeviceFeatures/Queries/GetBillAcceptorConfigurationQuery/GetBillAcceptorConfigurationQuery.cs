using System.Text.Json;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries;

public record GetBillAcceptorConfigurationQuery : IRequest<JsonDocument>
{
    public string? CurrencyIsoCode { get; set; }
}

internal sealed class GetBillAcceptorConfigurationQueryHandler : IRequestHandler<GetBillAcceptorConfigurationQuery, JsonDocument>
{
    private readonly ITerminalRepository _db;
    private readonly IDeviceDriverFactory _deviceDriverFactory;

    public GetBillAcceptorConfigurationQueryHandler(ITerminalRepository db, IDeviceDriverFactory deviceDriverFactory)
    {
        _db = db;
        _deviceDriverFactory = deviceDriverFactory;
    }

    public async Task<JsonDocument> Handle(GetBillAcceptorConfigurationQuery request, CancellationToken cancellationToken)
    {
        var config = await _db.GetBillAcceptorConfigurationAsync();
        return _deviceDriverFactory.ConvertConfigurationToJson(config);
    }
}