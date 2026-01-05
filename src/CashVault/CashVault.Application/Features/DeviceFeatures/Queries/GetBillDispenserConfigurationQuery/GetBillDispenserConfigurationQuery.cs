using System.Text.Json;
using System.Text.Json.Nodes;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public record GetBillDispenserConfigurationQuery : IRequest<JsonDocument?> { }

    internal sealed class GetBillDispenserConfigurationQueryHandler : IRequestHandler<GetBillDispenserConfigurationQuery, JsonDocument?>
    {
        private readonly ITerminalRepository _db;
        private readonly IMoneyStatusRepository _moneyStatusRepository;
        private readonly IDeviceDriverFactory _deviceDriverFactory;

        public GetBillDispenserConfigurationQueryHandler(IDeviceDriverFactory deviceDriverFactory, ITerminalRepository db, IMoneyStatusRepository moneyStatusRepository)
        {
            _deviceDriverFactory = deviceDriverFactory;
            _db = db;
            _moneyStatusRepository = moneyStatusRepository;
        }

        public async Task<JsonDocument?> Handle(GetBillDispenserConfigurationQuery request, CancellationToken cancellationToken)
        {
            var config = await _db.GetBillDispenserConfigurationAsync();
            var dispenserBillCountStatus = await _moneyStatusRepository.GetDispenserBillCountStatusAsync();
            var result = _deviceDriverFactory.ConvertConfigurationToJson(config);

            if (result != null)
            {
                var isEmpty = !dispenserBillCountStatus.Cassettes.Any(x => x.CurrentBillCount > 0);

                var jsonNode = JsonNode.Parse(result.RootElement.GetRawText())!.AsObject();

                jsonNode["isEmpty"] = isEmpty;

                return JsonDocument.Parse(jsonNode.ToJsonString());
            }

            return result;
        }
    }
}
