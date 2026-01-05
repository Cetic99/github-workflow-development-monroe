using CashVault.Application.Common;
using CashVault.Domain.Aggregates.DeviceAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.CreditFeatures.Commands;

public record AcceptBillCommand : ISynchronizationCommand, IRequest<decimal>
{
    public byte DenomKey { get; init; }
    public List<SingleAcceptorBillDenomination> AvailableDenominations { get; init; }

    public AcceptBillCommand(byte denomKey, List<SingleAcceptorBillDenomination> availableDenominations)
    {
        DenomKey = denomKey;
        AvailableDenominations = availableDenominations;
    }
}

public class AcceptBillCommandHandler : IRequestHandler<AcceptBillCommand, decimal>
{
    public async Task<decimal> Handle(AcceptBillCommand request, CancellationToken cancellationToken)
    {

        var paperBill = request.AvailableDenominations.FirstOrDefault(d => d.DataKey == request.DenomKey && d.IsEnabled);

        if (paperBill != null)
        {
            return paperBill.DataValue;
        }

        return 0;
    }
}

public class AcceptBillCommandValidator : AbstractValidator<AcceptBillCommand>
{
    public AcceptBillCommandValidator()
    {
        RuleFor(v => v.DenomKey).NotEmpty();
        RuleFor(v => v.AvailableDenominations).NotNull().Must(v => v.Count > 0);
    }
}