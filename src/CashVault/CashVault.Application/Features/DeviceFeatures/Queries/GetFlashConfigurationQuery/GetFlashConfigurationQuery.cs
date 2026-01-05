using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetFlashConfigurationQueryValidator : AbstractValidator<GetFlashConfigurationQuery>
    {
        public GetFlashConfigurationQueryValidator()
        {

        }
    }

    public record GetFlashConfigurationQuery : IRequest<FlashConfigurationDto>
    {

    }

    internal sealed class GetFlashConfigurationQueryHandler : IRequestHandler<GetFlashConfigurationQuery, FlashConfigurationDto>
    {
        private readonly ITerminalRepository _db;

        public GetFlashConfigurationQueryHandler(ITerminalRepository db)
        {
            _db = db;
        }

        public async Task<FlashConfigurationDto> Handle(GetFlashConfigurationQuery request, CancellationToken cancellationToken)
        {
            var dto = new FlashConfigurationDto();
            var config = await _db.GetFlashConfigurationAsync();

            dto.TopMessage = config.TopMessage;
            dto.ThemeToShow = config.ThemeToShow;

            return await Task.FromResult(dto);
        }
    }
}
