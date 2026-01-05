using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.OperatorFeatures.Commands;

public class BlockIdentificationCardCommand : IRequest<Unit>
{
    public int OperatorId { get; set; }
    public int CardId { get; set; }
    public string? Reason { get; set; }
}

public class BlockIdentificationCardCommandValidator : AbstractValidator<BlockIdentificationCardCommand>
{
    public BlockIdentificationCardCommandValidator()
    {
        RuleFor(x => x.OperatorId).NotEmpty().WithMessage("OperatorId is required.");
        RuleFor(x => x.CardId).NotEmpty().WithMessage("CardId is required.");
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500).WithMessage("Reason is required and should not exceed 500 characters.");
    }
}

internal sealed class BlockIdentificationCardCommandHandler : IRequestHandler<BlockIdentificationCardCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public BlockIdentificationCardCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(BlockIdentificationCardCommand request, CancellationToken cancellationToken)
    {
        Operator @operator = await _unitOfWork.OperatorRepository.GetOperatorWithIdentificationCardByCardId(request.OperatorId, request.CardId)
            ?? throw new ArgumentNullException();

        IdentificationCard card = @operator.IdentificationCards.FirstOrDefault(c => c.Id == request.CardId)
            ?? throw new ArgumentNullException();

        @operator.BlockCard(card.Guid, request.Reason ?? "Unknown reason");

        _unitOfWork.OperatorRepository.UpdateEntity(card);

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
