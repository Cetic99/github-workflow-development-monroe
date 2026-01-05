using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.OperatorFeatures.Commands;

public class DeactivateIdentificationCardCommand : IRequest<Unit>
{
    public int OperatorId { get; set; }
    public int CardId { get; set; }
    public string? Remarks { get; set; }
}

public class DeactivateIdentificationCardCommandValidator : AbstractValidator<DeactivateIdentificationCardCommand>
{
    public DeactivateIdentificationCardCommandValidator()
    {
        RuleFor(x => x.OperatorId).NotEmpty().WithMessage("OperatorId is required.");
        RuleFor(x => x.CardId).NotEmpty().WithMessage("CardId is required.");
    }
}

internal sealed class DeactivateIdentificationCardCommandHandler : IRequestHandler<DeactivateIdentificationCardCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateIdentificationCardCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeactivateIdentificationCardCommand request, CancellationToken cancellationToken)
    {
        Operator @operator = await _unitOfWork.OperatorRepository.GetOperatorWithIdentificationCardByCardId(request.OperatorId, request.CardId)
            ?? throw new ArgumentNullException();

        IdentificationCard card = @operator.IdentificationCards.FirstOrDefault(c => c.Id == request.CardId)
            ?? throw new ArgumentNullException();

        @operator.CloseCard(card.Guid, request.Remarks ?? string.Empty);

        _unitOfWork.OperatorRepository.UpdateEntity(card);

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
