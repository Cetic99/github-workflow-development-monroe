using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.TransactionAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.LogsFeatures.Queries;

public class GetMoneyStatusTransactionQueryValidator : AbstractValidator<GetMoneyStatusTransactionQuery>
{
    public GetMoneyStatusTransactionQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(-1);
        RuleFor(x => x.PageSize).NotEmpty();
    }
}

public record GetMoneyStatusTransactionQuery : IRequest<MoneyStatusTransactionLogsDto>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public DateTime? TimestampFrom { get; set; }
    public DateTime? TimestampTo { get; set; }
    public decimal? AmountFrom { get; set; }
    public decimal? AmountTo { get; set; }
    public int? TypeId { get; set; }
}

internal sealed class GetMoneyStatusTransactionQueryHandler : IRequestHandler<GetMoneyStatusTransactionQuery, MoneyStatusTransactionLogsDto>
{
    private readonly ITransactionRepository _db;
    private readonly IRegionalService _regionalService;
    private readonly ITerminal _terminal;

    public GetMoneyStatusTransactionQueryHandler(ITransactionRepository db, IRegionalService regionalService, ITerminal terminal)
    {
        _db = db;
        _regionalService = regionalService;
        _terminal = terminal;
    }

    public async Task<MoneyStatusTransactionLogsDto> Handle(GetMoneyStatusTransactionQuery req, CancellationToken cancellationToken)
    {
        var dto = new MoneyStatusTransactionLogsDto();
        var timeZone = _terminal.LocalTimeZone;

        var result = await _db.GetMoneyStatusTransactionsAsync(req.Page, req.PageSize, req.TypeId, req.TimestampFrom, req.TimestampTo, req.AmountFrom, req.AmountTo);

        if (result != null)
        {
            dto.MoneyStatusTransactionTypes = Enumeration.GetAll<MoneyStatusTransactionType>().ToList();

            dto.Page = result.Page;
            dto.PageSize = result.PageSize;
            dto.TotalCount = result.TotalCount;
            dto.Transactions = result.Data.Select(x => new MoneyStatusTransactionDto()
            {
                Id = x.Id,
                Amount = x.Amount,
                Timestamp = GetLocaleDateTime(x.Timestamp),
                Status = x.Status,
                Type = x.Type,
                Currency = x.Currency,
                DeviceType = x.DeviceType
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
