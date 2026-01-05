using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.TransactionAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.CreditFeatures.EventHandlers;

public class CoinRejectedEventHandler : INotificationHandler<CoinRejectedEvent>
{
    private readonly ILogger<CoinRejectedEventHandler> logger;
    private readonly INotificationService notificationService;

    public CoinRejectedEventHandler(ILogger<CoinRejectedEventHandler> logger, INotificationService notificationService)
    {
        this.logger = logger;
        this.notificationService = notificationService;
    }

    public async Task Handle(CoinRejectedEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.CoinRejected();

        logger.LogDebug("CoinRejectedEvent Handled");
    }
}
