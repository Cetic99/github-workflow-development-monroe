using CashVault.Application.Common.Models;
using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Application.Features.LogsFeatures.Queries;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.TransactionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CashVault.Infrastructure.PersistentStorage;

public class TransactionRepository : BaseRepository, ITransactionRepository
{
    private readonly ILogger<TransactionRepository> logger;

    public TransactionRepository(CashVaultContext dbContext, ILogger<TransactionRepository> logger) : base(dbContext)
    {
        this.logger = logger;
    }

    public async Task<Transaction> GetPendingTicketTransactionAsync(Guid ticketGuid)
    {
        var result = await _dbContext.Transactions.OfType<TicketTransaction>().Where(t => t.Status.Id == TransactionStatus.Pending.Id && t.Ticket.Guid == ticketGuid).ToListAsync();

        if (result.Count > 1)
        {
            throw new InvalidOperationException("Multiple pending transactions found for the same ticket.");
        }

        return result.FirstOrDefault();
    }

    public async Task<Transaction?> GetPendingTicketTransactionByExternalReference(string barcode)
    {
        var result = await _dbContext.Transactions.OfType<TicketTransaction>().Where(t => t.Status.Id == TransactionStatus.Pending.Id && t.ExternalReference == barcode).ToListAsync();

        if (result.Count > 1)
        {
            throw new InvalidOperationException("Multiple pending transactions found for the same ticket.");
        }

        return result.FirstOrDefault();
    }

    public async Task<Transaction> GetTransactionByGuidAsync(Guid transactionGuid)
    {
        return await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Guid == transactionGuid);
    }

    public async Task<Transaction> GetTransactionByIdAsync(int transactionId)
    {
        return await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
    }

    public async Task<MoneyStatusTransaction?> GetMoneyStatusTransactionByIdAsync(int transactionGuid)
    {
        return await _dbContext.MoneyStatusTransactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == transactionGuid);
    }

    public async Task<PaginatedResultSet<MoneyStatusTransactionDto>> GetMoneyStatusTransactionsAsync(int page = 0,
    int pageSize = 10,
    int? typeId = null,
    DateTime? timestampFrom = null,
    DateTime? timestampTo = null,
    decimal? amountFrom = null,
    decimal? amountTo = null)
    {
        var moneyStatusQuery = _dbContext.MoneyStatusTransactions.AsQueryable();

        if (timestampFrom.HasValue)
        {
            moneyStatusQuery = moneyStatusQuery.Where(t => t.Timestamp >= timestampFrom);
        }

        if (timestampTo.HasValue)
        {
            moneyStatusQuery = moneyStatusQuery.Where(t => t.Timestamp <= timestampTo);
        }

        if (amountFrom.HasValue)
        {
            moneyStatusQuery = moneyStatusQuery.Where(t => t.Amount >= amountFrom);
        }

        if (amountTo.HasValue)
        {
            moneyStatusQuery = moneyStatusQuery.Where(t => t.Amount <= amountTo);
        }

        if (typeId.HasValue && typeId > 0)
            moneyStatusQuery = moneyStatusQuery.Where(t => t.Type.Id == typeId.Value);

        var resultList = await moneyStatusQuery
            .OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResultSet<MoneyStatusTransactionDto>
        {
            Data = resultList.Select(x => new MoneyStatusTransactionDto()
            {
                Id = x.Id,
                Amount = x.Amount,
                Timestamp = x.Timestamp,
                Status = x.Status,
                Type = x.Type,
                Currency = x.Currency,
                DeviceType = x.DeviceType.Code,
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = await moneyStatusQuery.CountAsync()
        };
    }

    public async Task<PaginatedResultSet<TransactionDto>> GetTransactionsAsync
        (string? transactionGuid = "",
         DateTime? processingStartedDateFrom = null,
         DateTime? processingStartedDateTo = null,
         DateTime? processingEndedDateFrom = null,
         DateTime? processingEndedDateTo = null,
         int? typeId = null,
         string? kind = null,
         int? statusId = null,
         decimal? amountFrom = null,
         decimal? amountTo = null,
         decimal? amountRequestedFrom = null,
         decimal? amountRequestedTo = null,
         int page = 0,
         int pageSize = 10)
    {

        var transactionsQuery = _dbContext.Transactions.Select(t => new
        {
            t.Id,
            t.Guid,
            t.Amount,
            t.AmountRequested,
            t.PreviousCreditAmount,
            t.NewCreditAmount,
            t.ProcessingStarted,
            t.ProcessingEnded,
            t.Status,
            t.Type,
            t.Currency,
            Kind = EF.Property<string>(t, "TransactionKind"),
            IsMoneyStatusTransaction = (bool)false

        }).AsQueryable();

        var expression = transactionsQuery;


        if (!string.IsNullOrEmpty(transactionGuid))
        {
            expression = expression.Where(t => t.Guid.Equals(transactionGuid));
        }


        if (processingStartedDateFrom.HasValue)
        {
            expression = expression.Where(t => t.ProcessingStarted >= processingStartedDateFrom);
        }

        if (processingStartedDateTo.HasValue)
        {
            expression = expression.Where(t => t.ProcessingStarted <= processingStartedDateTo);
        }

        if (processingEndedDateFrom.HasValue)
        {
            expression = expression.Where(t => t.ProcessingEnded >= processingEndedDateFrom);
        }

        if (!string.IsNullOrEmpty(kind))
        {
            expression = expression.Where(t => t.Kind == kind);
        }

        if (processingEndedDateTo.HasValue)
        {
            expression = expression.Where(t => t.ProcessingEnded <= processingEndedDateTo);
        }

        if (amountFrom.HasValue)
        {
            expression = expression.Where(t => t.Amount >= amountFrom);
        }

        if (amountTo.HasValue)
        {
            expression = expression.Where(t => t.Amount <= amountTo);
        }

        if (amountRequestedFrom.HasValue)
            expression = expression.Where(t => t.AmountRequested >= amountRequestedFrom);

        if (amountRequestedTo.HasValue)
            expression = expression.Where(t => t.AmountRequested <= amountRequestedTo);

        if (typeId.HasValue && typeId > 0)
            expression = expression.Where(t => t.Type.Id == typeId.Value);

        if (statusId.HasValue && statusId > 0)
            expression = expression.Where(t => t.Status.Id == statusId.Value);

        var resultList = await expression
                    .OrderByDescending(x => x.ProcessingStarted)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

        return new PaginatedResultSet<TransactionDto>
        {
            Data = resultList.Select(x => new TransactionDto()
            {
                Id = x.Id,
                Amount = x.Amount,
                AmountRequested = x.AmountRequested,
                PreviousCreditAmount = x.PreviousCreditAmount,
                NewCreditAmount = x.NewCreditAmount,
                Status = x.Status,
                Type = x.Type,
                ProcessingEnded = x.ProcessingEnded,
                ProcessingStarted = x.ProcessingStarted,
                Currency = x.Currency,
                Kind = x.Kind,
                IsMoneyStatusTransaction = x.IsMoneyStatusTransaction
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = await expression.CountAsync()
        };
    }

    public async Task<List<Transaction>> GetBillTransactionsAync(DateTime toDate, DateTime? fromDate = null)
    {
        try
        {
            var data = await
            _dbContext.Transactions
                .Where(x => x.ProcessingEnded.HasValue &&
                             (!fromDate.HasValue || x.ProcessingEnded.Value >= fromDate.Value) &&
                             x.ProcessingEnded.Value <= toDate &&
                            (EF.Property<string>(x, "TransactionKind") == nameof(AcceptorBillTransaction) ||
                             EF.Property<string>(x, "TransactionKind") == nameof(DispenserBillTransaction)))
                .ToListAsync();

            return data;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Query [GetBillTransactionsAync] failed");
            return new List<Transaction>();
        }
    }

    public async Task<List<TicketTransaction>> GetTicketTransactionsAync(DateTime toDate, DateTime? fromDate = null)
    {
        try
        {
            var data = await
            _dbContext.Transactions
                .Where(x => x.ProcessingEnded.HasValue &&
                             (!fromDate.HasValue || x.ProcessingEnded.Value >= fromDate.Value) &&
                             x.ProcessingEnded.Value <= toDate &&
                             EF.Property<string>(x, "TransactionKind") == nameof(TicketTransaction))
                .Cast<TicketTransaction>()
                .Include(x => x.Ticket)
                .ToListAsync();

            return data;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Query [GetTicketTransactionsAync] failed");
            return new List<TicketTransaction>();
        }
    }

    public Task<bool> SaveTransactionAsync(Transaction transaction)
    {
        _dbContext.Transactions.Update(transaction);
        return Task.FromResult(true);
    }
}
