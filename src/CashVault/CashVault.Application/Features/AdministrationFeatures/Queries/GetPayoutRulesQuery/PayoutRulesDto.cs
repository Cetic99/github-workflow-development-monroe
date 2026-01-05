namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class PayoutRulesDto
    {
        public PayoutRuleDto Tickets { get; set; }
        public PayoutRuleDto Bills { get; set; }
        public PayoutRuleDto Coins { get; set; }
    }

    public class PayoutRuleDto
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }
}