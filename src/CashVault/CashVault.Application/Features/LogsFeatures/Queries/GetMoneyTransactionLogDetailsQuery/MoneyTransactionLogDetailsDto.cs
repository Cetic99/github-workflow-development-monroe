using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.LogsFeatures.Queries
{
    public class MoneyTransactionLogDetailsDto
    {
        public int Id { get; set; }
        public decimal? Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public Currency? Currency { get; set; }
        public string? Kind { get; set; }
        public string DeviceType { get; set; }
        public List<BillDispenserCassetteItemDto> DispenserMoneyStatus { get; set; }
        public AcceptorItemDto? AcceptorMoneyStatus { get; set; }
        public decimal OldDeviceBillAmount { get; set; }
        public decimal NewDeviceBillAmount { get; set; }

        public MoneyTransactionLogDetailsDto()
        {
            DispenserMoneyStatus = [];
        }
    }
}
