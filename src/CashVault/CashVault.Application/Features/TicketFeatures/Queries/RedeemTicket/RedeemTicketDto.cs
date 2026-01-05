using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.TicketFeatures.Queries;

public class RedeemTicketDto
{
    public string? Barcode { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUsed { get; set; }

    public bool IsValid { get; set; } // Computed property to indicate if the ticket is valid
    public Currency? Currency { get; set; }

    public RedeemTicketDto()
    {
        TotalAmount = 0;
        DateCreated = DateTime.MinValue;
        IsValid = false;
    }
}
