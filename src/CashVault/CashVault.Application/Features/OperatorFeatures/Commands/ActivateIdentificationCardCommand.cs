using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.OperatorFeatures.Commands;

public class ActivateIdentificationCardCommand : IRequest<Unit>
{
    public int OperatorId { get; set; }
    public int CardId { get; set; }
    public string? Remarks { get; set; }
}

public class ActivateIdentificationCardCommandValidator : AbstractValidator<ActivateIdentificationCardCommand>
{
    public ActivateIdentificationCardCommandValidator()
    {
        RuleFor(x => x.OperatorId).NotEmpty().WithMessage("OperatorId is required.");
        RuleFor(x => x.CardId).NotEmpty().WithMessage("CardId is required.");
    }
}

internal sealed class ActivateIdentificationCardCommandHandler : IRequestHandler<ActivateIdentificationCardCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivateIdentificationCardCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActivateIdentificationCardCommand request, CancellationToken cancellationToken)
    {
        Operator @operator = await _unitOfWork.OperatorRepository.GetOperatorWithIdentificationCardByCardId(request.OperatorId, request.CardId)
            ?? throw new ArgumentNullException();

        IdentificationCard card = @operator.IdentificationCards.FirstOrDefault(c => c.Id == request.CardId)
            ?? throw new ArgumentNullException();

        @operator.ActivateCard(card.Guid, request.Remarks);

        _unitOfWork.OperatorRepository.UpdateEntity<IdentificationCard>(card);

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
