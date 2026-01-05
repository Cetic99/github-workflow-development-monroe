using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.LogsFeatures.Queries
{
    public class MoneyStatusTransactionDto
    {
        public int Id { get; set; }
        public Guid? Guid { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Timestamp { get; set; }
        public TransactionStatus Status { get; set; }
        public MoneyStatusTransactionType Type { get; set; }
        public Currency? Currency { get; set; }
        public string DeviceType { get; set; }
    }
}
