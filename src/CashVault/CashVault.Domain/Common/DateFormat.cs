namespace CashVault.Domain.Common
{

    public class DateFormat : Enumeration
    {
        public static readonly DateFormat DotDMY = new(1, "DD.MM.YYYY.");
        public static readonly DateFormat DashMDY = new(2, "MM-DD-YYYY");
        public static readonly DateFormat SlashYMD = new(3, "YYYY/MM/DD");

        public static readonly DateFormat DefaultDateFormat = DotDMY;

        public DateFormat(int id, string code)
            : base(id, code)
        { }
    }
}
