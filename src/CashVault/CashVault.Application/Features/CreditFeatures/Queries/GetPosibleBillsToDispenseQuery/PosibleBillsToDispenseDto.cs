using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Application.Features.CreditFeatures.Queries
{
    public class PosibleBillsToDispenseDto
    {
        public decimal MinimumAmount { get; set; }
        public decimal MaximumAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencySymbol { get; set; }
        public List<DenominationDispenseOption> DenominationDispenseOptions { get; set; }
        public List<DenominationCount> PrefilledCombinations { get; set; }
        public decimal PrefilledAmountToPrint { get; set; }
        public int AmountPrecision { get; set; }

        public PosibleBillsToDispenseDto()
        {
            DenominationDispenseOptions = [];
        }

        public PosibleBillsToDispenseDto(decimal minAmount, decimal maxAmount, decimal totalAmount, Currency currency, List<DenominationDispenseOption> denominationDispenseOptions, List<DenominationCount> prefilledCombinations, decimal prefilledAmountToPrint, int amountPrecision)
        {
            DenominationDispenseOptions = denominationDispenseOptions ?? [];
            MinimumAmount = minAmount;
            MaximumAmount = maxAmount;
            TotalAmount = totalAmount;
            CurrencySymbol = currency.Symbol;
            PrefilledCombinations = prefilledCombinations ?? [];
            PrefilledAmountToPrint = prefilledAmountToPrint;
            AmountPrecision = amountPrecision;
        }
    }
}
