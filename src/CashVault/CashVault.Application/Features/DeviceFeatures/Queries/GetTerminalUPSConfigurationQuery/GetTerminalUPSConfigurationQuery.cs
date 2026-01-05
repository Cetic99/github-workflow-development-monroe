using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetTerminalUPSConfigurationQueryValidator : AbstractValidator<GetTerminalUPSConfigurationQuery>
    {
        public GetTerminalUPSConfigurationQueryValidator()
        {

        }
    }

    public record GetTerminalUPSConfigurationQuery : IRequest<TerminalUPSConfigurationDto>
    {

    }

    internal sealed class GetTerminalUPSConfigurationQueryHandler : IRequestHandler<GetTerminalUPSConfigurationQuery, TerminalUPSConfigurationDto>
    {
        private readonly ITerminalRepository _db;

        public GetTerminalUPSConfigurationQueryHandler(ITerminalRepository db)
        {
            _db = db;
        }

        public async Task<TerminalUPSConfigurationDto> Handle(GetTerminalUPSConfigurationQuery request, CancellationToken cancellationToken)
        {
            var dto = new TerminalUPSConfigurationDto();
            var config = await _db.GetCurrentUpsConfigurationAsync();

            dto.ConfiguredUpsType = config.ConfiguredUpsType;
            dto.ShutdownDelay = config.ShutdownDelay;
            dto.StartupDelay = config.StartupDelay;
            dto.UptimeAfterPowerShortage = config.UptimeAfterPowerShortage;

            return await Task.FromResult(dto);
        }
    }
}
