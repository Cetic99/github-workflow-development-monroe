using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateMessageCommand : IRequest<Unit>
{
    public string? LanguageCode { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
}

public class UpdateMessageCommandHandler(IUnitOfWork unitOfWork, ITerminal terminal, IRegionalService regionalService)
    : IRequestHandler<UpdateMessageCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMessageCommand command, CancellationToken cancellationToken)
    {
        var message = await unitOfWork.MessageRepository.GetMessageAsync(command.LanguageCode, command.Key);

        if (message == null) throw new ArgumentNullException("message");

        message.SetValue(command.Value);
        unitOfWork.MessageRepository.UpdateEntity(message);
        unitOfWork.SaveChanges();

        return Unit.Value;
    }
}
public class UpdateMessageCommandValidator : AbstractValidator<UpdateMessageCommand>
{
    public UpdateMessageCommandValidator(ILocalizer t)
    {
        RuleFor(x => x.LanguageCode).NotEmpty().WithMessage(t["The language code is required."]);
        RuleFor(x => x.Key).NotEmpty().WithMessage(t["The key field is required."]);
        RuleFor(x => x.Value).NotEmpty().WithMessage(t["The value field is required"]);
    }
}