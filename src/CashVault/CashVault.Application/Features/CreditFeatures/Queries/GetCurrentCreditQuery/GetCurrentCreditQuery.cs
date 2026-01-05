using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.CreditFeatures.Queries;

public record GetCurrentCreditQuery : IRequest<CurrentCreditDTO>
{

}

internal sealed class GetMessagesQueryHandler : IRequestHandler<GetCurrentCreditQuery, CurrentCreditDTO>
{
    private readonly IMoneyStatusRepository _db;
    private readonly ILogger _logger;

    public GetMessagesQueryHandler(IMoneyStatusRepository db, ILogger<GetMessagesQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<CurrentCreditDTO> Handle(GetCurrentCreditQuery request, CancellationToken cancellationToken)
    {
        var dto = new CurrentCreditDTO();

        var creditStatus = await _db.GetCurrentCreditStatusAsync();

        dto.Currency = creditStatus?.Currency ?? Currency.Default;
        dto.CreditAmount = creditStatus?.Amount ?? 0;

        return await Task.FromResult(dto);
    }
}

public class GetCurrentCreditQueryValidator : AbstractValidator<GetCurrentCreditQuery>
{
    public GetCurrentCreditQueryValidator()
    {

    }
}