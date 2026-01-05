using CashVault.Domain.Aggregates.TicketAggregate;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class TicketLogsDto
    {
        public List<TicketDto> Tickets { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<TicketType> TicketTypes { get; set; }

        public TicketLogsDto()
        {
            Tickets = new List<TicketDto>();
        }
    }
}