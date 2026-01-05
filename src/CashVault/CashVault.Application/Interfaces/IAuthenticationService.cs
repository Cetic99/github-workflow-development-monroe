using CashVault.Domain.Aggregates.OperatorAggregate;

namespace CashVault.Application.Interfaces;

/// <summary>
/// Interface abstracting..
/// </summary>
public interface IAuthenticationService
{
    string HashPasword(string password, out byte[] salt);

    bool VerifyPassword(Operator @opeartor, string passwordToVerify);
}