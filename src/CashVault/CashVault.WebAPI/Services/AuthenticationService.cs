using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using System.Security.Cryptography;
using System.Text;

namespace CashVault.WebAPI.Services;

public class AuthenticationService : IAuthenticationService
{
    const int keySize = 64;
    const int iterations = 350000;
    HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

    private readonly IConfiguration _configuration;

    public AuthenticationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string HashPasword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(keySize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            hashAlgorithm,
            keySize);

        return Convert.ToHexString(hash);
    }

    public bool VerifyPassword(Operator @opeartor, string passwordToVerify)
    {
        var hashToCompare =
            Rfc2898DeriveBytes
                .Pbkdf2(passwordToVerify, Convert.FromHexString(@opeartor.PasswordSalt), iterations, hashAlgorithm, keySize);

        return CryptographicOperations
                    .FixedTimeEquals(hashToCompare, Convert.FromHexString(@opeartor.Password));
    }
}