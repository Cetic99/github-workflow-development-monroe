using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using CashVault.Domain.TransactionAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class GetTransactionLogQueryValidator : AbstractValidator<GetTransactionLogQuery>
    {
        public GetTransactionLogQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThan(-1);
            RuleFor(x => x.PageSize).NotEmpty();
        }
    }

    public record GetTransactionLogQuery : IRequest<TransactionLogsDto>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public DateTime? ProcessingStartedDateFrom { get; set; }
        public DateTime? ProcessingStartedDateTo { get; set; }
        public DateTime? ProcessingEndedDateFrom { get; set; }
        public DateTime? ProcessingEndedDateTo { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public decimal? AmountRequestedFrom { get; set; }
        public decimal? AmountRequestedTo { get; set; }
        public int? TypeId { get; set; }
        public string? Kind { get; set; }
        public int? StatusId { get; set; }
    }

    internal sealed class GetTransactionLogQueryHandler : IRequestHandler<GetTransactionLogQuery, TransactionLogsDto>
    {
        private readonly ITransactionRepository _db;
        private readonly IRegionalService _regionalService;
        private readonly ITerminal _terminal;

        public GetTransactionLogQueryHandler(ITransactionRepository db, IRegionalService regionalService, ITerminal terminal)
        {
            _db = db;
            _regionalService = regionalService;
            _terminal = terminal;
        }

        public async Task<TransactionLogsDto> Handle(GetTransactionLogQuery request, CancellationToken cancellationToken)
        {
            var dto = new TransactionLogsDto();
            var timeZone = _terminal.LocalTimeZone;

            var result = await _db.GetTransactionsAsync
                (page: request.Page,
                 pageSize: request.PageSize,
                 processingStartedDateFrom: request.ProcessingStartedDateFrom,
                 processingStartedDateTo: request.ProcessingStartedDateTo,
                 processingEndedDateFrom: request.ProcessingEndedDateFrom,
                 processingEndedDateTo: request.ProcessingEndedDateTo,
                 amountFrom: request.AmountFrom,
                 amountTo: request.AmountTo,
                 amountRequestedFrom: request.AmountRequestedFrom,
                 amountRequestedTo: request.AmountRequestedTo,
                 typeId: request.TypeId,
                 kind: request.Kind,
                 statusId: request.StatusId);

            if (result != null)
            {
                dto.TransactionTypes = Enumeration.GetAll<TransactionType>().ToList();
                dto.TransactionStatuses = Enumeration.GetAll<TransactionStatus>().ToList();

                dto.Page = result.Page;
                dto.PageSize = result.PageSize;
                dto.TotalCount = result.TotalCount;
                dto.Transactions = result.Data.Select(x => new TransactionDto()
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    AmountRequested = x.AmountRequested,
                    PreviousCreditAmount = x.PreviousCreditAmount,
                    NewCreditAmount = x.NewCreditAmount,
                    Status = x.Status,
                    Type = x.Type,
                    ProcessingEnded = GetLocaleDateTime(x.ProcessingEnded),
                    ProcessingStarted = GetLocaleDateTime(x.ProcessingStarted),
                    Currency = x.Currency,
                    Kind = x.Kind,
                    IsMoneyStatusTransaction = x.IsMoneyStatusTransaction
                })
                .ToList();
            }

            return await Task.FromResult(dto);
        }

        private DateTime? GetLocaleDateTime(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return null;

            return _regionalService.ConvertToUserTimeZone(dateTime.Value, _terminal.LocalTimeZone);
        }
    }
}
