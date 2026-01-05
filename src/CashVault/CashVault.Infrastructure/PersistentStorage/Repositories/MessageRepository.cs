using CashVault.Application.Common.Models;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.MessageAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CashVault.Infrastructure.PersistentStorage;

public class MessageRepository : BaseRepository, IMessageRepository
{
    private readonly CashVaultContext _dbContext;
    private readonly ILogger<CashVaultContext> _logger;

    public MessageRepository(CashVaultContext dbContext, ILogger<CashVaultContext> logger) : base(dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<PaginatedResultSet<Message>> GetMessagesAsync(int page, int pageSize, string? key, string? value, string? languagCode)
    {
        var expression = _dbContext.Messages.AsQueryable();

        if (!string.IsNullOrEmpty(key))
            expression = expression.Where(x => x.Key != null && x.Key.ToLower().Contains(key.ToLower()));

        if (!string.IsNullOrEmpty(value))
            expression = expression.Where(x => x.Value != null && x.Value.ToLower().Contains(value.ToLower()));

        if (!string.IsNullOrEmpty(languagCode))
            expression = expression.Where(x => x.LanguageCode != null && x.LanguageCode.ToLower().Contains(languagCode.ToLower()));

        var resultList = await expression.OrderByDescending(x => x.Created)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

        return new PaginatedResultSet<Message>
        {
            Data = resultList,
            Page = page,
            PageSize = pageSize,
            TotalCount = await expression.CountAsync()
        };
    }

    public async Task<List<Message>> GetAllMessagesAsync()
    {
        return await _dbContext.Messages.ToListAsync();
    }

    public async Task<Message> GetMessageAsync(string languageCode, string key)
    {
        return await
            _dbContext.Messages.FirstOrDefaultAsync
                (x => x.Key.Equals(key) && x.LanguageCode.Equals(languageCode));
    }
}