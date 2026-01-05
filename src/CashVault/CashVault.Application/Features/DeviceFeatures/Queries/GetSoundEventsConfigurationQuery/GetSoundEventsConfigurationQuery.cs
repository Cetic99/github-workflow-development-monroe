using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetSoundEventsConfigurationQueryValidator : AbstractValidator<GetSoundEventsConfigurationQuery>
    {
        public GetSoundEventsConfigurationQueryValidator()
        {

        }
    }

    public record GetSoundEventsConfigurationQuery : IRequest<SoundEventsConfigurationDto>
    {

    }

    internal sealed class GetSoundEventsConfigurationQueryHandler : IRequestHandler<GetSoundEventsConfigurationQuery, SoundEventsConfigurationDto>
    {
        private readonly ITerminalRepository _db;

        public GetSoundEventsConfigurationQueryHandler(ITerminalRepository db)
        {
            _db = db;
        }

        public async Task<SoundEventsConfigurationDto> Handle(GetSoundEventsConfigurationQuery request, CancellationToken cancellationToken)
        {
            var dto = new SoundEventsConfigurationDto();
            var config = await _db.GetSoundEventsConfigurationAsync();

            if (config.SoundEvents.Any()) 
                dto.SoundEvents = 
                    config.SoundEvents.DistinctBy(x => x.Id).Select(x => new SoundEventDto() {
                        Id = x.Id,
                        Name = x.Name,  
                        SoundTypeCode = x.SoundTypeCode,
                    }).ToList();

            return await Task.FromResult(dto);
        }
    }
}
