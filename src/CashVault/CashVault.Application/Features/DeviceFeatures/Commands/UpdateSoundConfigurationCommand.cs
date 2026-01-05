using FluentValidation;
using MediatR;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateSoundConfigurationCommand : IRequest<Unit>
{
    public bool? Enabled { get; set; }
}

public class UpdateSoundConfigurationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSoundConfigurationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateSoundConfigurationCommand command, CancellationToken cancellationToken)
    {
        var config = new SoundConfiguration();

        config.Enabled = (bool) command.Enabled;
        
        unitOfWork.TerminalRepository.UpdateSoundConfigurationAsync(config);
        await unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
public class UpdateSoundConfigurationCommandValidator : AbstractValidator<UpdateSoundConfigurationCommand>
{
    public UpdateSoundConfigurationCommandValidator()
    {
        RuleFor(x => x.Enabled).NotEmpty();
    }
}
