using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.ParcelLockerFeatures.Queries;

public class DeliveryOptionDto
{
    public string Code { get; init; } = string.Empty;
    public bool PaymentRequired { get; init; }
    public decimal? Amount { get; init; }
    public Currency? Currency { get; init; }
}
