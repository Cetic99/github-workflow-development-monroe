using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate
{
    public class MoneyStatusTransactionType : Enumeration
    {
        public static readonly MoneyStatusTransactionType Refill = new(1, nameof(Refill));
        public static readonly MoneyStatusTransactionType Harvest = new(2, nameof(Harvest));

        public MoneyStatusTransactionType(int id, string code) : base(id, code)
        {
        }
    }
}
