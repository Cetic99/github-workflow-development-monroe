using CashVault.Domain.ValueObjects;
namespace CashVault.Application.Features.CreditFeatures.Queries;

public class CurrentCreditDTO
{
    public decimal CreditAmount { get; set; }
    public Currency Currency { get; set; }
}
