using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public Guid? Guid { get; set; }
        public decimal? AmountRequested { get; set; }
        public decimal? Amount { get; set; }
        public decimal PreviousCreditAmount { get; set; } = 0;
        public decimal NewCreditAmount { get; set; } = 0;
        public DateTime? ProcessingStarted { get; set; }
        public DateTime? ProcessingEnded { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionType Type { get; set; }
        public Currency? Currency { get; set; }
        public string? Kind { get; set; }
        public bool IsMoneyStatusTransaction { get; set; }
    }
}