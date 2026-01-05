namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration
{
    public class PayoutRulesConfiguration
    {
        public PayoutRule Tickets { get; set; }
        public PayoutRule Bills { get; set; }
        public PayoutRule Coins { get; set; }

        public PayoutRulesConfiguration()
        {
            Bills = new PayoutRule() { Min = 0, Max = 1000 };
            Tickets = new PayoutRule() { Min = 0, Max = 1000 };
            Coins = new PayoutRule() { Min = 0, Max = 1000 };
        }
    }

    public class PayoutRule
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }
}
