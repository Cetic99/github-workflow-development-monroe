namespace CashVault.Infrastructure.Configuration
{
    public class JWTSettings
    {
        public string Secret { get; set; } = string.Empty;
        public int AccessTokenValidityInSeconds { get; set; } = 3600; // Default to 1 hour
        public int RefreshTokenValidityInSeconds { get; set; } = 86400; // Default to 1 day
    }
}
