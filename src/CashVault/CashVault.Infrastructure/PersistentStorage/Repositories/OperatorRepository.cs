using System.Security.Cryptography;
using System.Text;
using CashVault.Application.Common.Models;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.OperatorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CashVault.Infrastructure.PersistentStorage;

public class OperatorRepository : BaseRepository, IOperatorRepository
{
    const int keySize = 64;
    const int iterations = 350000;
    HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
    private readonly IConfiguration configuration;

    public OperatorRepository(CashVaultContext dbContext, IConfiguration configuration) : base(dbContext)
    {
        this.configuration = configuration;
    }

    public async Task<List<Permission>> GetPermissionsAsync()
    {
        return await _dbContext.Permissions.ToListAsync();
    }

    public async Task<Operator> GetOperatorWithPermissionsAsync(int id)
    {
        return await
            _dbContext.Operators
                .Include(x => x.OperatorPermissions)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
    }

    public async Task<PaginatedResultSet<Operator>> GetOperatorsAsync(int page, int pageSize)
    {
        var expression = _dbContext.Operators.AsQueryable();

        var resultList = await expression
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

        return new PaginatedResultSet<Operator>
        {
            Data = resultList,
            Page = page,
            PageSize = pageSize,
            TotalCount = await expression.CountAsync()
        };
    }

    public async Task<PaginatedResultSet<Operator>> GetOperatorsWithPermissionsAsync(
        int page,
        int pageSize,
        string? firstName = null,
        string? lastName = null)
    {
        try
        {
            var expression =
                _dbContext.Operators
                    .Include(x => x.OperatorPermissions)
                    .AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
                expression = expression.Where(x => x.FirstName != null && x.FirstName.ToLower().Contains(firstName.ToLower()));

            if (!string.IsNullOrEmpty(lastName))
                expression = expression.Where(x => x.LastName != null && x.LastName.ToLower().Contains(lastName.ToLower()));

            var resultList = await expression
                .OrderByDescending(x => x.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResultSet<Operator>
            {
                Data = resultList,
                Page = page,
                PageSize = pageSize,
                TotalCount = await expression.CountAsync()
            };
        }
        catch (Exception ex)
        {
            return new PaginatedResultSet<Operator>
            {
                Data = new List<Operator>(),
                Page = page,
                PageSize = pageSize,
                TotalCount = 0
            };
        }

        return new PaginatedResultSet<Operator>
        {
            Data = new List<Operator>(),
            Page = page,
            PageSize = pageSize,
            TotalCount = 0
        };
    }

    public async Task<Operator?> GetOperatorWithIdentificationCards(int id)
    {
        return await
            _dbContext.Operators
                .Where(x => x.Id == id)
                .Include(x => x.IdentificationCards)
                .Include(x => x.OperatorPermissions)
                .FirstOrDefaultAsync();
    }

    public async Task<Operator?> GetOperatorWithIdentificationCardByCardId(int operatorId, int cardId)
    {
        return await
            _dbContext.Operators
                .Where(x => x.Id == operatorId)
                .Include(x => x.IdentificationCards.Where(c => c.Id == cardId))
                .FirstOrDefaultAsync();
    }

    public async Task<Operator> GetOperatorByUsernameAsync(string username)
    {
        return await
            _dbContext.Operators
                .Include(x => x.OperatorPermissions)
                .Where(x => x.Username.Equals(username))
                .FirstOrDefaultAsync();
    }

    public async Task<PaginatedResultSet<IdentificationCard>> GetOperatorIdentificationCardsAsync(int operatorId, int page, int pageSize)
    {
        try
        {
            var expression = _dbContext.IdentificationCards.AsQueryable();

            var now = DateTime.UtcNow;

            //if (operatorId > 0)
            //    expression = expression
            //        .Where(t => t.OperatorId == operatorId &&
            //                t.ValidFrom <= now &&
            //                (!t.ValidTo.HasValue || t.ValidTo.Value > now));

            if (operatorId > 0)
                expression = expression
                    .Where(t => t.OperatorId == operatorId);

            var resultList = await expression
                .OrderByDescending(x => x.IssuedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResultSet<IdentificationCard>
            {
                Data = resultList,
                Page = page,
                PageSize = pageSize,
                TotalCount = await expression.CountAsync()
            };
        }
        catch (Exception ex)
        {
            var x = 1;
        }

        return new PaginatedResultSet<IdentificationCard>
        {
            Data = new List<IdentificationCard>(),
            Page = page,
            PageSize = pageSize,
            TotalCount = 0
        };
    }

    public async Task<IdentificationCard?> GetIdentificationCardByUuidAndUIDAsync(Guid cardUuid, string cardUID)
    {
        return await
            _dbContext.IdentificationCards
                .Where(x => x.Guid.Equals(cardUuid) && x.CardIdentifier.Equals(cardUID))
                .FirstOrDefaultAsync();
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

    public async Task<bool> VerifyPasswordAsync(Operator op, string enteredPasword)
    {
        var hashToCompare =
            Rfc2898DeriveBytes
                .Pbkdf2(enteredPasword, Convert.FromHexString(op.PasswordSalt), iterations, hashAlgorithm, keySize);

        return CryptographicOperations
                    .FixedTimeEquals(hashToCompare, Convert.FromHexString(op.Password));
    }

    public string GetDefaultPassword()
    {
        return configuration["DefaultPassword"];
    }
}