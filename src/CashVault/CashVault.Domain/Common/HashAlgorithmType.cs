namespace CashVault.Domain.Common
{
    public class HashAlgorithmType : Enumeration
    {
        public static readonly HashAlgorithmType SHA256 = new(1, "SHA-256");
        public static readonly HashAlgorithmType SHA512 = new(2, "SHA-512");

        public static readonly HashAlgorithmType Default = SHA512;

        HashAlgorithmType(int id, string name) : base(id, name) { }
    }
}
