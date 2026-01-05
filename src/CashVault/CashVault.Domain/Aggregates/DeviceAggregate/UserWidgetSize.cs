using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.DeviceAggregate
{
    public class UserWidgetSize : Enumeration
    {
        public static readonly UserWidgetSize Small = new(1, "S");
        public static readonly UserWidgetSize Medium = new(2, "M");
        public static readonly UserWidgetSize Large = new(3, "L");

        public static readonly UserWidgetSize DefaultWidgetSize = UserWidgetSize.Medium;

        public UserWidgetSize(int id, string code)
            : base(id, code) { }
    }
}
