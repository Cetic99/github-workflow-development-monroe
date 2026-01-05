using CashVault.Application.Common.Models;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;

namespace CashVault.Application.Interfaces.Persistence
{
    public interface IEventLogRepository
    {
        public Task<PaginatedResultSet<BaseEvent>> GetEventLogsAsync(int page, int pageSize, string? message, string? name, EventLogType? type, DeviceType? deviceType, DateTime? timestampFrom, DateTime? timestampTo);

        public Task<PaginatedResultSet<DeviceFailEvent>> GetFailEventLogsAsync(int page, int pageSize, string? message, string? name, DeviceType? deviceType, DateTime? timestampFrom, DateTime? timestampTo);

        Task<int> GetTicketRejectedEventLogCountAsync(DateTime toDate, DateTime? fromDate = null);

        Task<int> GetBillRejectedEventLogCountAsync(DateTime toDate, DateTime? fromDate = null);

        Task<BaseEvent> GetLastShiftMoneyHarvestedEventAsync();

        Task<List<BaseEvent>> GetUnsentEvents(int batchSize = 100);

        void UpdateEvents(List<BaseEvent> events);
    }
}
