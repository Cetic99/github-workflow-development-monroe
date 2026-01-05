using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.ParcelLockerFeatures.QueryModels;

public class ShipmentDto
{
    public string Barcode { get; set; } = string.Empty;
    public string ParcelLockerSize { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public DateTime ExpirationDate { get; set; }

    public bool PaymentRequired => Amount > 0;
    public Currency Currency { get; set; } = Currency.Default;
}
