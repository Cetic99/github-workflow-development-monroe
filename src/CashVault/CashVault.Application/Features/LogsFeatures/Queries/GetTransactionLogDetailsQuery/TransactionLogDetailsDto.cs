using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class TransactionLogDetailsDto
    {
        public int Id { get; set; }
        public decimal? AmountRequested { get; set; }
        public decimal? Amount { get; set; }
        public decimal PreviousCreditAmount { get; set; } = 0;
        public decimal NewCreditAmount { get; set; } = 0;
        public DateTime? ProcessingStarted { get; set; }
        public DateTime? ProcessingEnded { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public Currency? Currency { get; set; }
        public string? ExternalReference { get; set; }
        public string? Description { get; set; }
        public string? Kind { get; set; }
        public bool IsCms { get; set; }

        public List<DispenserBillTransactionItemDto> DispenserBillItems { get; set; }

        public TransactionLogDetailsDto()
        {
            DispenserBillItems = new List<DispenserBillTransactionItemDto>();
        }
    }
}