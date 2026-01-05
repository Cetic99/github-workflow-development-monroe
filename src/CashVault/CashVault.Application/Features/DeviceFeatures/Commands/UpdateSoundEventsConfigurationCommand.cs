using FluentValidation;
using MediatR;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateSoundEventsConfigurationCommand : IRequest<Unit>
{
    public List<SoundEvent> SoundEvents { get; set; }
}

public class UpdateSoundEventsConfigurationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSoundEventsConfigurationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateSoundEventsConfigurationCommand command, CancellationToken cancellationToken)
    {
        var config = new SoundEventsConfiguration();

        config.SoundEvents = command.SoundEvents;
        
        unitOfWork.TerminalRepository.UpdateSoundEventsConfigurationAsync(config);
        await unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
public class UpdateSoundEventsConfigurationCommandValidator : AbstractValidator<UpdateSoundEventsConfigurationCommand>
{
    public UpdateSoundEventsConfigurationCommandValidator()
    {
        RuleFor(x => x.SoundEvents).NotEmpty();
    }
}
