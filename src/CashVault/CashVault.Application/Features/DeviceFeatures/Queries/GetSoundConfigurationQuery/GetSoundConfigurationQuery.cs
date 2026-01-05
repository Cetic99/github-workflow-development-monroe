using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetSoundConfigurationQueryValidator : AbstractValidator<GetSoundConfigurationQuery>
    {
        public GetSoundConfigurationQueryValidator()
        {

        }
    }

    public record GetSoundConfigurationQuery : IRequest<SoundConfigurationDto>
    {

    }

    internal sealed class GetSoundConfigurationQueryHandler : IRequestHandler<GetSoundConfigurationQuery, SoundConfigurationDto>
    {
        private readonly ITerminalRepository _db;

        public GetSoundConfigurationQueryHandler(ITerminalRepository db)
        {
            _db = db;
        }

        public async Task<SoundConfigurationDto> Handle(GetSoundConfigurationQuery request, CancellationToken cancellationToken)
        {
            var dto = new SoundConfigurationDto();
            var config = await _db.GetSoundConfigurationAsync();

            dto.Enabled = config.Enabled;

            return await Task.FromResult(dto);
        }
    }
}
