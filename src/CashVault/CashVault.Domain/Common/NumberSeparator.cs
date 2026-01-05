namespace CashVault.Domain.Common
{
    public class NumberSeparator : Enumeration
    {
        public static readonly NumberSeparator Dot = new(1, "Dot", ".");
        public static readonly NumberSeparator Comma = new(2, "Comma", ",");
        public static readonly NumberSeparator Space = new(3, "Space", " ");

        public string Symbol { get; private set; }

        public NumberSeparator(
            int id,
            string code,
            string symbol)
            : base(id, code)
        {
            Symbol = symbol;
        }
    }
}
