namespace CashVault.Domain.Aggregates.OperatorAggregate
{
    public static class PermissionEnum
    {
        public const string BillAcceptor = "BillAcceptor";
        public const string BillDispenser = "BillDispenser";
        public const string CoinDispenser = "CoinDispenser";
        public const string CardReader = "CardReader";
        public const string MoneyService = "MoneyService";
        public const string Reports = "Reports";
        public const string Administration = "Administration";
        public const string Configuration = "Configuration";
        public const string Shutdown = "Shutdown";
        public const string Maintenance = "Maintenance";
        public const string Logs = "Logs";
    }
}