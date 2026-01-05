using FluentValidation;
using MediatR;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateFlashConfigurationCommand : IRequest<Unit>
{
    public int ThemeToShow { get; set; }
    public string TopMessage { get; set; }
}

public class UpdateFlashConfigurationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateFlashConfigurationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateFlashConfigurationCommand command, CancellationToken cancellationToken)
    {
        var config = new FlashConfiguration();

        config.ThemeToShow = command.ThemeToShow;
        config.TopMessage = command.TopMessage;

        unitOfWork.TerminalRepository.UpdateFlashConfigurationAsync(config);
        await unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
public class UpdateFlashConfigurationCommandValidator : AbstractValidator<UpdateFlashConfigurationCommand>
{
    public UpdateFlashConfigurationCommandValidator()
    {
        RuleFor(x => x.ThemeToShow).NotEmpty();
        RuleFor(x => x.TopMessage).NotEmpty();
    }
}
