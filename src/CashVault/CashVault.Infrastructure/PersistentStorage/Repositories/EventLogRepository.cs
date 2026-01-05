using CashVault.Application.Common.Models;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;
using CashVault.Domain.Aggregates.OperatorAggregate.Events;
using CashVault.Domain.Aggregates.TicketAggregate.Events;
using CashVault.Domain.Common.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CashVault.Infrastructure.PersistentStorage.Repositories
{
    public class EventLogRepository : IEventLogRepository
    {
        private readonly CashVaultContext _dbContext;
        private readonly ILogger<CashVaultContext> _logger;

        public EventLogRepository(CashVaultContext dbContext, ILogger<CashVaultContext> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<PaginatedResultSet<BaseEvent>> GetEventLogsAsync(int page, int pageSize, string? message, string? name, Domain.Common.EventLogType? type, DeviceType deviceType, DateTime? timestampFrom, DateTime? timestampTo)
        {
            var expression = _dbContext.EventLogs.AsQueryable();

            if (timestampFrom.HasValue)
                expression = expression.Where(t => t.Created.Date >= timestampFrom.Value.Date);

            if (timestampTo.HasValue)
                expression = expression.Where(t => t.Created.Date <= timestampTo.Value.Date);

            if (!string.IsNullOrEmpty(message))
                expression = expression.Where(x => x.Message != null && x.Message.ToLower().Contains(message.ToLower()));

            if (!string.IsNullOrEmpty(name))
                expression = expression.Where(x => x.EventName != null && x.EventName.ToLower().Contains(name.ToLower()));

            if (type != null)
                expression = expression.Where(x => EF.Property<string>(x, "EventType") == type.Code);

            if (deviceType != null)
            {
                expression = expression.OfType<DeviceEvent>()
                    .Where(x => EF.Property<string>(x, "DeviceType") == deviceType.Code);
            }

            var resultList = await expression.OrderByDescending(x => x.Created)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            return new PaginatedResultSet<BaseEvent>
            {
                Data = resultList,
                Page = page,
                PageSize = pageSize,
                TotalCount = await expression.CountAsync()
            };
        }

        public async Task<PaginatedResultSet<DeviceFailEvent>> GetFailEventLogsAsync(int page, int pageSize, string? message, string? name, DeviceType deviceType, DateTime? timestampFrom, DateTime? timestampTo)
        {
            var expression = _dbContext.EventLogs
                .AsQueryable()
                .OfType<DeviceFailEvent>();

            if (timestampFrom.HasValue)
                expression = expression.Where(t => t.Created.Date >= timestampFrom.Value.Date);

            if (timestampTo.HasValue)
                expression = expression.Where(t => t.Created.Date <= timestampTo.Value.Date);

            if (!string.IsNullOrEmpty(message))
                expression = expression.Where(x => x.Message != null && x.Message.ToLower().Contains(message.ToLower()));

            if (!string.IsNullOrEmpty(name))
                expression = expression.Where(x => x.EventName != null && x.EventName.ToLower().Contains(name.ToLower()));

            if (deviceType != null)
                expression = expression.Where(x => EF.Property<string>(x, "DeviceType") == deviceType.Code);

            try
            {
                var resultList = await expression.OrderByDescending(e => e.Created)
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync();

                return new PaginatedResultSet<DeviceFailEvent>
                {
                    Data = resultList,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = await expression.CountAsync()
                };
            }
            catch (Exception ex)
            {
                var x = ex;
            }

            return new PaginatedResultSet<DeviceFailEvent>
            {

            };
        }

        public async Task<int> GetTicketRejectedEventLogCountAsync(DateTime toDate, DateTime? fromDate = null)
        {
            return await
                _dbContext.EventLogs
                    .Where(x => (x.EventName != null && x.EventName.Equals(nameof(TicketRejectedEvent))) &&
                                (fromDate == null || x.Created.Date >= fromDate.Value.Date) &&
                                 x.Created <= toDate.Date)
                    .CountAsync();
        }

        public async Task<BaseEvent> GetLastShiftMoneyHarvestedEventAsync()
        {
            try
            {
                return await
                    _dbContext.EventLogs
                        .Where(x => x.EventName != null && x.EventName.Equals(nameof(OperatorShiftMoneyHarvestedEvent)))
                        .OrderByDescending(x => x.Created)
                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Query [GetLastShiftMoneyHarvestedEventAsync] failed");
                return null;
            }
        }

        public async Task<int> GetBillRejectedEventLogCountAsync(DateTime toDate, DateTime? fromDate = null)
        {
            return await
                _dbContext.EventLogs
                    .Where(x => (x.EventName != null && x.EventName.Equals(nameof(BillRejectedEvent))) &&
                                (fromDate == null || x.Created.Date >= fromDate.Value.Date) &&
                                 x.Created <= toDate.Date)
                    .CountAsync();
        }

        public async Task<List<BaseEvent>> GetUnsentEvents(int batchSize = 100)
        {
            var result = await _dbContext.EventLogs
                    .Where(e => (EF.Property<string>(e, "EventType") == Domain.Common.EventLogType.DeviceEvent.Code
                    || EF.Property<string>(e, "EventType") == Domain.Common.EventLogType.TransactionEvent.Code)
                    && !e.IsSentToRemote)
                    .Take(batchSize)
                    .ToListAsync();

            if (result != null)
            {
                return result;
            }
            else
            {
                return [];
            }
        }

        public void UpdateEvents(List<BaseEvent> events)
        {
            if (events == null || events.Count == 0)
            {
                return;
            }
            _dbContext.EventLogs.UpdateRange(events);
        }
    }
}
