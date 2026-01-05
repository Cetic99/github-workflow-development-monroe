using CashVault.Application.Interfaces.Persistence;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.OperatorFeatures.Queries.GetOperatorIdentificationCardsQuery;

public class GetOperatorIdentificationCardsQueryValidator : AbstractValidator<GetOperatorIdentificationCardsQuery>
{
    public GetOperatorIdentificationCardsQueryValidator()
    {
        RuleFor(x => x.OperatorId).NotEmpty().WithMessage("OperatorId is required.");
        RuleFor(x => x.Page).NotNull().WithMessage("Page is required.");
        RuleFor(x => x.PageSize).NotEmpty().WithMessage("PageSize is required.");
    }
}

public record GetOperatorIdentificationCardsQuery : IRequest<OperatorIdCardsDto>
{
    public int OperatorId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

internal sealed class GetOperatorIdentificationCardsQueryHandler : IRequestHandler<GetOperatorIdentificationCardsQuery, OperatorIdCardsDto>
{
    private readonly IOperatorRepository _db;

    public GetOperatorIdentificationCardsQueryHandler(IOperatorRepository db)
    {
        _db = db;
    }

    public async Task<OperatorIdCardsDto> Handle(GetOperatorIdentificationCardsQuery request, CancellationToken cancellationToken)
    {
        var dto = new OperatorIdCardsDto();
        var operators = await _db.GetOperatorIdentificationCardsAsync(request.OperatorId, request.Page, request.PageSize);

        if (operators?.Data?.Any() ?? false)
        {
            dto.Page = operators.Page;
            dto.PageSize = operators.PageSize;
            dto.TotalCount = operators.TotalCount;

            dto.Cards = operators.Data.Select(x => new IdentificationCardDto()
            {
                Id = x.Id,
                SerialNumber = x.SerialNumber,
                IssuedAt = x.IssuedAt,
                IssuedBy = x.IssuedBy,
                ValidFrom = x.ValidFrom,
                ValidTo = x.ValidTo,
                LastStatusChange = x.LastStatusChange,
                Status = x.Status,
                Remarks = x.Remarks,
                OperatorId = x.OperatorId
            }).ToList();
        }

        return await Task.FromResult(dto);
    }
}
