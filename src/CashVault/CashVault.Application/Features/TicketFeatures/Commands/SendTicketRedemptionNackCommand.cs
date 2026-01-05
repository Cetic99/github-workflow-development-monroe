using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.TicketFeatures.Commands;

public class SendTicketRedemptionNackCommand : IRequest<bool>
{
    public string Barcode { get; init; } = null!;
    public TicketType TicketType { get; init; } = null!;
    public Guid Id { get; set; }

    public SendTicketRedemptionNackCommand(string barcode, TicketType ticketType, Guid id)
    {
        Barcode = barcode;
        TicketType = ticketType;
        Id = id;
    }
}

public class SendTicketRedemptionNackCommandHandler : IRequestHandler<SendTicketRedemptionNackCommand, bool>
{
    private readonly ILogger<SendTicketRedemptionNackCommandHandler> _logger;
    private readonly ITicketProviderFactory _ticketProviderFactory;

    public SendTicketRedemptionNackCommandHandler(ILogger<SendTicketRedemptionNackCommandHandler> logger,
                                                  ITicketProviderFactory ticketProviderFactory)
    {
        _logger = logger;
        _ticketProviderFactory = ticketProviderFactory;
    }

    public async Task<bool> Handle(SendTicketRedemptionNackCommand command, CancellationToken cancellationToken)
    {
        var ticketProvider = _ticketProviderFactory.GetTicketProviderByType(command.TicketType);

        try
        {
            var result = await ticketProvider.RedeemTicketNack(command.Barcode, command.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending NACK for ticket with barcode {command.Barcode}. [Provider: {command.TicketType}]");
            throw;
        }
    }
}