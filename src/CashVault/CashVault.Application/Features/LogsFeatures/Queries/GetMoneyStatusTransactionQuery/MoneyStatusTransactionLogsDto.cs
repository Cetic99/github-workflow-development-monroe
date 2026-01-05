using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.TransactionAggregate;

namespace CashVault.Application.Features.LogsFeatures.Queries;

public class MoneyStatusTransactionLogsDto
{
    public List<MoneyStatusTransactionDto> Transactions { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<MoneyStatusTransactionType> MoneyStatusTransactionTypes { get; set; }

    public MoneyStatusTransactionLogsDto()
    {
        Transactions = [];
    }
}
