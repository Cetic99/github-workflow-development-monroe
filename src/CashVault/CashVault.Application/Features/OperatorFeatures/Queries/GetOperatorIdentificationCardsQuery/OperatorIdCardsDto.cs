using CashVault.Domain.Aggregates.OperatorAggregate;

namespace CashVault.Application.Features.OperatorFeatures.Queries.GetOperatorIdentificationCardsQuery;

public class OperatorIdCardsDto
{
    public List<IdentificationCardDto> Cards { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public OperatorIdCardsDto()
    {
        Cards = new List<IdentificationCardDto>();
        Page = 0;
        PageSize = 10;
        TotalCount = 0;
    }
}