using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdatePayoutRulesCommand : IRequest<Unit>
{
    public decimal BillsMin { get; set; }
    public decimal BillsMax { get; set; }
    public decimal CoinsMin { get; set; }
    public decimal CoinsMax { get; set; }
    public decimal TicketsMin { get; set; }
    public decimal TicketsMax { get; set; }
}

public class UpdatePayoutRulesCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePayoutRulesCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePayoutRulesCommand command, CancellationToken cancellationToken)
    {
        var config = new PayoutRulesConfiguration()
        {
            Tickets = new PayoutRule() { Min = command.TicketsMin, Max = command.TicketsMax },
            Bills = new PayoutRule() { Min = command.BillsMin, Max = command.BillsMax },
            Coins = new PayoutRule() { Min = command.CoinsMin, Max = command.CoinsMax }
        };

        unitOfWork.TerminalRepository.UpdatePayoutRulesConfigurationAsync(config);
        await unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
public class UpdatePayoutRulesCommandValidator : AbstractValidator<UpdatePayoutRulesCommand>
{
    public UpdatePayoutRulesCommandValidator(ILocalizer t)
    {
        RuleFor(r => r.BillsMax).GreaterThan(0).WithMessage(t["Bills max must be greater than 0."]);
        RuleFor(r => r.CoinsMax).GreaterThan(0).WithMessage(t["Coins max must be greater than 0."]);
        RuleFor(r => r.TicketsMax).GreaterThan(0).WithMessage(t["Tickets max must be greater than 0."]);
        RuleFor(r => r.BillsMin).LessThanOrEqualTo(r => r.BillsMax).WithMessage(t["Bills min must be less than or equal to bills max."]);
        RuleFor(r => r.CoinsMin).LessThanOrEqualTo(r => r.CoinsMax).WithMessage(t["Coins min must be less than or equal to coins max."]);
        RuleFor(r => r.TicketsMin).LessThanOrEqualTo(r => r.TicketsMax).WithMessage(t["Tickets min must be less than or equal to tickets max."]);
        RuleFor(r => r.BillsMin).GreaterThanOrEqualTo(0).WithMessage(t["Bills min must be greater than or equal to 0."]);
        RuleFor(r => r.TicketsMin).GreaterThanOrEqualTo(0).WithMessage(t["Tickets min must be greater than or equal to 0."]);
        RuleFor(r => r.CoinsMin).GreaterThanOrEqualTo(0).WithMessage(t["Coins min must be greater than or equal to 0."]);
    }
}