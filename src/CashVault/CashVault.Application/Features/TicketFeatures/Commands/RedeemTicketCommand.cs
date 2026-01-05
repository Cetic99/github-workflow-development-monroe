using CashVault.Domain.Aggregates.TicketAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.TicketFeatures.Commands;

public class RedeemTicketCommand : IRequest<bool>
{
    public string Barcode { get; init; } = null!;
    public TicketType TicketType { get; init; } = null!;
    public Guid Id { get; init; } = Guid.NewGuid();
}

public class RedeemTicketCommandValidator : AbstractValidator<RedeemTicketCommand>
{
    public RedeemTicketCommandValidator()
    {
        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("Barcode is required.")
            .Matches(@"^\d{8,20}$").WithMessage("Barcode must be between 8 and 20 digits.");
        RuleFor(x => x.TicketType)
            .NotNull().WithMessage("TicketType is required.");
    }
}

internal class RedeemTicketCommandHandler : IRequestHandler<RedeemTicketCommand, bool>
{
    private readonly ISender _mediator;

    public RedeemTicketCommandHandler(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task<bool> Handle(RedeemTicketCommand command, CancellationToken cancellationToken)
    {
        if (command.TicketType == TicketType.BetboxTicket)
        {
            // Not supported
        }
        else if (command.TicketType == TicketType.TITO)
        {
            return await _mediator.Send(new RedeemTitoTicketCommand(command.Barcode), cancellationToken);
        }

        return false;
    }
}
