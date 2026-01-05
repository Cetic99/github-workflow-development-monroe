namespace CashVault.Infrastructure.CMSService
{
    internal class CMSResponseCodes
    {
        public static int Valid = 0;
        public static int Unknown = 1;
        public static int AlreadyRedeemed = 2;
        public static int Expired = 3;
        public static int Invalid = 4;
        public static int OperationFailed = 100;
    }
}
