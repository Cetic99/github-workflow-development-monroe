using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate
{
    public class PaymentOption : Enumeration
    {
        public static readonly PaymentOption Cash = new(1, nameof(Cash));
        public static readonly PaymentOption Card = new(2, nameof(Card));
        public static readonly PaymentOption Coupon = new(3, nameof(Coupon));

        public PaymentOption(int id, string code)
            : base(id, code) { }
    }
}
