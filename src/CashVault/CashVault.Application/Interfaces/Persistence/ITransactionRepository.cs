using CashVault.Application.Common.Models;
using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Application.Features.LogsFeatures.Queries;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.TransactionAggregate;

namespace CashVault.Application.Interfaces.Persistence;

public interface ITransactionRepository
{
    Task<Transaction> GetTransactionByGuidAsync(Guid transactionId);
    Task<Transaction> GetPendingTicketTransactionAsync(Guid ticketGuid);
    Task<Transaction> GetPendingTicketTransactionByExternalReference(string barcode);
    Task<Transaction> GetTransactionByIdAsync(int transactionId);
    Task<bool> SaveTransactionAsync(Transaction transaction);
    Task<PaginatedResultSet<TransactionDto>> GetTransactionsAsync
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
         int pageSize = 10);

    Task<PaginatedResultSet<MoneyStatusTransactionDto>> GetMoneyStatusTransactionsAsync(int page, int pageSize, int? typeId, DateTime? timestampFrom, DateTime? timestampTo, decimal? amountFrom, decimal? amountTo);

    Task<List<Transaction>> GetBillTransactionsAync(DateTime toDate, DateTime? fromDate = null);

    Task<List<TicketTransaction>> GetTicketTransactionsAync(DateTime toDate, DateTime? fromDate = null);

    Task<MoneyStatusTransaction> GetMoneyStatusTransactionByIdAsync(int transactionGuid);
}
