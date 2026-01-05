using CashVault.Application.Common.Models;
using CashVault.Domain.Aggregates.TicketAggregate;

namespace CashVault.Application.Interfaces.Persistence;

public interface ITicketRepository
{
    Task<Ticket> GetTicketByGuidAsync(Guid ticketGuid);
    Task<Ticket> GetTicketByIdAsync(int ticketId);
    Task<Ticket> GetTicketByBarcodeAsync(string barcode);
    Task<bool> SaveTicketAsync(Ticket ticket);
    Task<bool> DeleteTicketAsync(Ticket ticket);
    Task<PaginatedResultSet<Ticket>> GetTicketsAsync
            (string? barcode = "",
             string? ticketGuid = "",
             string? number = "",
             decimal? amountFrom = null,
             decimal? amountTo = null,
             bool? isLocal = null,
             bool? isPrinted = null,
             bool? isRedeemed = null,
             bool? isStacked = null,
             bool? isExpired = null,
             int? daysValidFrom = null,
             int? daysValidTo = null,
             DateTime? printingRequestedAtFrom = null,
             DateTime? printingRequestedAtTo = null,
             DateTime? printingCompletedAtFrom = null,
             DateTime? printingCompletedAtTo = null,
             int? typeId = null,
             int page = 1,
             int pageSize = 10);
    bool DoesBarcodeExist(string barcode);
}
