using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class TicketDto
    {
        public int Id { get; set; }
        public decimal? Amount { get; set; }
        public string? Barcode { get; set; }
        public TicketType Type { get; set; }
        public string? Number { get; set; }
        public DateTime? Created { get; set; }
        public Currency? Currency { get; set; }

    }
}