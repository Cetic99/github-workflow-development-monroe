namespace CashVault.Infrastructure.Configuration
{
    public class HeartbeatSettings
    {
        public string PrivateKeyPath { get; set; } = string.Empty;
        public string HashAlgorithm { get; set; } = string.Empty;
        public string SignatureAlgorithm { get; set; } = string.Empty;
    }
}
