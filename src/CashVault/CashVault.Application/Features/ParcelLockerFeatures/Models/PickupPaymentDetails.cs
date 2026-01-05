using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.ParcelLockerFeatures.Models;

public class PickupPaymentDetails
{
    public bool PaymentRequired { get; set; }
    public decimal? Amount { get; set; }
    public Currency? Currency { get; set; }
}
