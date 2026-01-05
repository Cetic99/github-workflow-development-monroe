using CashVault.Application.Interfaces.Persistence;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public record GetTerminalServerConfigurationQuery : IRequest<TerminalServerConfigurationDto> { }

    internal sealed class GetTerminalServerConfigurationQueryHandler : IRequestHandler<GetTerminalServerConfigurationQuery, TerminalServerConfigurationDto>
    {
        private readonly ITerminalRepository _db;

        public GetTerminalServerConfigurationQueryHandler(ITerminalRepository db)
        {
            _db = db;
        }

        public async Task<TerminalServerConfigurationDto> Handle(GetTerminalServerConfigurationQuery request, CancellationToken cancellationToken)
        {
            var config = await _db.GetCurrentServerConfigurationAsync();

            return new TerminalServerConfigurationDto()
            {
                Url = config.ServerUrl,
                DeviceId = config.DeviceId,
                IsEnabled = config.IsEnabled,
                MinimalLogLevel = config.MinimalLogLevel
            };
        }
    }
}
