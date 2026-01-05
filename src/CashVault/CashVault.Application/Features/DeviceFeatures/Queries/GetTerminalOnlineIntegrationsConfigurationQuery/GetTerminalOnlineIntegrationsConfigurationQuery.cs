using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetTerminalOnlineIntegrationsConfigurationQueryValidator : AbstractValidator<GetTerminalOnlineIntegrationsConfigurationQuery>
    {
        public GetTerminalOnlineIntegrationsConfigurationQueryValidator()
        {

        }
    }

    public record GetTerminalOnlineIntegrationsConfigurationQuery : IRequest<TerminalOnlineIntegrationsConfigurationDto>
    {

    }

    internal sealed class GetTerminalOnlineIntegrationsConfigurationQueryHandler : IRequestHandler<GetTerminalOnlineIntegrationsConfigurationQuery, TerminalOnlineIntegrationsConfigurationDto>
    {
        private readonly ITerminalRepository _db;

        public GetTerminalOnlineIntegrationsConfigurationQueryHandler(ITerminalRepository db)
        {
            _db = db;
        }

        public async Task<TerminalOnlineIntegrationsConfigurationDto> Handle(GetTerminalOnlineIntegrationsConfigurationQuery request, CancellationToken cancellationToken)
        {
            var dto = new TerminalOnlineIntegrationsConfigurationDto();
            var config = await _db.GetCurrentOnlineIntegrationsConfigurationAsync();

            dto.Url = config.Url;
            dto.DeviceId = config.DeviceId;
            dto.SecretKey = config.SecretKey;
            dto.CasinoManagementSystem = config.CasinoManagementSystem;
            dto.TimeoutInSeconds = config.TimeoutInSeconds;

            return await Task.FromResult(dto);
        }
    }
}
