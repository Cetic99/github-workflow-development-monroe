using CashVault.Application.Common.Models;
using CashVault.Domain.Aggregates.OperatorAggregate;

namespace CashVault.Application.Interfaces.Persistence;

public interface IOperatorRepository : IBaseRepository<Operator>
{
    string GetDefaultPassword();
    Task<Operator> GetOperatorWithPermissionsAsync(int id);
    Task<PaginatedResultSet<Operator>> GetOperatorsAsync(int page, int pageSize);
    Task<PaginatedResultSet<Operator>> GetOperatorsWithPermissionsAsync(int page, int pageSize, string? firstName = null, string? lastName = null);
    Task<List<Permission>> GetPermissionsAsync();
    Task<Operator?> GetOperatorWithIdentificationCards(int id);
    Task<Operator> GetOperatorByUsernameAsync(string username);
    Task<bool> VerifyPasswordAsync(Operator op, string enteredPasword);
    string HashPasword(string password, out byte[] salt);
    Task<PaginatedResultSet<IdentificationCard>> GetOperatorIdentificationCardsAsync(int operatorId, int page, int pageSize);
    Task<IdentificationCard?> GetIdentificationCardByUuidAndUIDAsync(Guid cardUuid, string cardUID);
    Task<Operator?> GetOperatorWithIdentificationCardByCardId(int operatorId, int cardId);
}