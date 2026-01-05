using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.TicketFeatures.Queries;

public class RedeemTicketQuery : IRequest<RedeemTicketDto>
{
    public string Barcode { get; set; } = null!;
    public TicketType TicketType { get; set; } = null!;
    public Guid Id { get; set; }
}

public class RedeemTicketQueryValidator : AbstractValidator<RedeemTicketQuery>
{
    public RedeemTicketQueryValidator()
    {
        RuleFor(x => x.Barcode).NotNull().NotEmpty()
            .Must(b => b.Length >= 8 && b.Length <= 20)
            .WithMessage("Barcode must be between 8 and 20 digits.");
        RuleFor(x => x.TicketType).NotNull().WithMessage("TicketType is required.");
    }
}

internal sealed class RedeemTicketQueryHandler : IRequestHandler<RedeemTicketQuery, RedeemTicketDto>
{
    private readonly ITicketProviderFactory _ticketProviderFactory;
    private readonly ILogger<RedeemTicketQueryHandler> _logger;

    public RedeemTicketQueryHandler(ITicketProviderFactory ticketProviderFactory, ILogger<RedeemTicketQueryHandler> logger)
    {
        _ticketProviderFactory = ticketProviderFactory;
        _logger = logger;
    }

    public async Task<RedeemTicketDto> Handle(RedeemTicketQuery query, CancellationToken cancellationToken)
    {
        var ticketProvider = _ticketProviderFactory.GetTicketProviderByType(query.TicketType);
        var result = new RedeemTicketDto();

        try
        {
            result = await ticketProvider.RedeemTicket(query.Barcode, query.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while geting response for geting ticket info for provider: {query.TicketType.Code} and ticket with barcode: {query.Barcode}");
        }

        return result;
    }
}
