using CashVault.Application.Common.Models;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.TicketAggregate;
using Microsoft.EntityFrameworkCore;

namespace CashVault.Infrastructure.PersistentStorage
{
    public class TicketRepository : BaseRepository, ITicketRepository
    {
        public TicketRepository(CashVaultContext dbContext) : base(dbContext) { }

        public Task<bool> DeleteTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public bool DoesBarcodeExist(string barcode)
        {
            return _dbContext.Tickets.Any(t => t.Barcode == barcode);
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

        }

        public async Task<Ticket> GetTicketByBarcodeAsync(string barcode)
        {
            return await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Barcode == barcode);
        }

        public async Task<Ticket> GetTicketByGuidAsync(Guid ticketGuid)
        {
            return await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Guid == ticketGuid);
        }

        public async Task<PaginatedResultSet<Ticket>> GetTicketsAsync
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
             int pageSize = 10)
        {
            var now = DateTime.UtcNow;

            var expression = _dbContext.Tickets.AsQueryable();

            if (!string.IsNullOrEmpty(barcode))
                expression = expression.Where(t => t.Barcode.Contains(barcode));

            if (!string.IsNullOrEmpty(ticketGuid))
                expression = expression.Where(t => t.Guid.Equals(ticketGuid));

            if (!string.IsNullOrEmpty(number))
                expression = expression.Where(t => t.Number.Contains(number));

            if (amountFrom.HasValue)
                expression = expression.Where(t => t.Amount >= amountFrom);

            if (amountTo.HasValue)
                expression = expression.Where(t => t.Amount <= amountTo);

            if (daysValidFrom.HasValue)
                expression = expression.Where(t => t.DaysValid >= daysValidFrom);

            if (daysValidTo.HasValue)
                expression = expression.Where(t => t.DaysValid <= daysValidTo);

            if (printingRequestedAtFrom.HasValue)
                expression = expression.Where(t => t.PrintingRequestedAt >= printingRequestedAtFrom);

            if (printingRequestedAtTo.HasValue)
                expression = expression.Where(t => t.PrintingRequestedAt <= printingRequestedAtTo);

            if (printingCompletedAtFrom.HasValue)
                expression = expression.Where(t => t.PrintingCompletedAt >= printingCompletedAtFrom);

            if (printingCompletedAtTo.HasValue)
                expression = expression.Where(t => t.PrintingCompletedAt <= printingCompletedAtTo);

            if (isLocal.HasValue)
                expression = expression.Where(t => t.IsLocal == isLocal);

            if (isPrinted.HasValue)
                expression = expression.Where(t => t.IsPrinted == isPrinted);

            if (isRedeemed.HasValue)
                expression = expression.Where(t => t.IsRedeemed == isRedeemed);

            if (isStacked.HasValue)
                expression = expression.Where(t => t.IsStacked == isStacked);

            if (isExpired.HasValue)
                expression = expression.Where(t => t.ExpirationDateTime.Date <= now);

            if (typeId.HasValue && typeId > 0)
                expression = expression.Where(t => t.Type.Id == typeId);

            var resultList = await expression
                        .OrderByDescending(x => x.Created)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            return new PaginatedResultSet<Ticket>
            {
                Data = resultList,
                Page = page,
                PageSize = pageSize,
                TotalCount = await expression.CountAsync()
            };
        }

        public Task<bool> SaveTicketAsync(Ticket ticket)
        {
            _dbContext.Tickets.Update(ticket);
            return Task.FromResult(true);
        }
    }
}
