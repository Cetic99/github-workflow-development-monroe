using CashVault.Domain.Aggregates.TicketAggregate;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class DailyMediaReportDto
    {
        public int BillsDispensedCount { get; set; }
        public decimal BillsDispensedValue { get; set; }
        public int BillsAcceptedCount { get; set; }
        public decimal BillsAcceptedValue { get; set; }
        public int BillsRejectedByAcceptorCount { get; set; }
        public int BillsRejectedByDispenserCount { get; set; }

        public int TicketsPrintedCount { get; set; }
        public decimal TicketsPrintedValue { get; set; }
        public int TicketsAcceptedCount { get; set; }
        public decimal TicketsAcceptedValue { get; set; }
        public int TicketsRejectedByAcceptorCount { get; set; }
    }
}