namespace CashVault.Domain.Common
{
    public class SignatureAlgorithmType : Enumeration
    {
        public static readonly SignatureAlgorithmType RSA = new(1, "RSA");
        public static readonly SignatureAlgorithmType HMAC = new(2, "HMAC");


        public static readonly SignatureAlgorithmType Default = RSA;

        SignatureAlgorithmType(int id, string name) : base(id, name) { }
    }
}
