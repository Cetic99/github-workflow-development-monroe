using CashVault.Domain.ValueObjects;

namespace CashVault.Infrastructure.TicketProviders.Betbox.Dtos;

internal class RedeemTicketDto
{
    public string? TermId { get; set; }
    public string? Barcode { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUsed { get; set; }
    public int Status { get; set; }

    public bool IsValid { get; set; } // Computed property to indicate if the ticket is valid
    public Currency? Currency { get; set; }
}
