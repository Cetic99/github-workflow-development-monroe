using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.OperatorFeatures.EventHandlers;

public class OperatorAuthenticationFailedEventHandler : INotificationHandler<OperatorAuthenticationFailedEvent>
{
    private readonly ILogger<OperatorAuthenticationFailedEventHandler> logger;
    private readonly INotificationService notificationService;

    public OperatorAuthenticationFailedEventHandler(ILogger<OperatorAuthenticationFailedEventHandler> logger, INotificationService notificationService)
    {
        this.logger = logger;
        this.notificationService = notificationService;
    }

    public async Task Handle(OperatorAuthenticationFailedEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.AuthenicationFailed();

        logger.LogDebug("AuthenticationFailedEvent Handled");
    }
}