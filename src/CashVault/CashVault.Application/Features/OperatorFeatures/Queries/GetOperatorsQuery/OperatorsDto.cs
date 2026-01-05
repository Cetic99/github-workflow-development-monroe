namespace CashVault.Application.Features.OperatorFeatures.Queries.GetOperatorsQuery;

public class OperatorsDto
{
    public List<OperatorDto> Operators { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public OperatorsDto()
    {
        Operators = new List<OperatorDto>();
        Page = 0;
        PageSize = 10;
        TotalCount = 0;
    }
}