using CashVault.Application.Common.Models;
using CashVault.Domain.Aggregates.MessageAggregate;

namespace CashVault.Application.Interfaces.Persistence;

public interface IMessageRepository : IBaseRepository<Message>
{
    Task<PaginatedResultSet<Message>> GetMessagesAsync(int page, int pageSize, string? key, string? value, string? languageCode);
    Task<List<Message>> GetAllMessagesAsync();
    Task<Message> GetMessageAsync(string languageCode, string key);
}
