using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.OperatorFeatures.EventHandlers;

public class OperatorAuthenticatedEventHandler : INotificationHandler<OperatorAuthenticatedEvent>
{
    private readonly ILogger<OperatorAuthenticatedEventHandler> logger;
    private readonly INotificationService notificationService;
    private readonly IUnitOfWork unitOfWork;

    public OperatorAuthenticatedEventHandler(ILogger<OperatorAuthenticatedEventHandler> logger, INotificationService notificationService, IUnitOfWork unitOfWork)
    {
        this.logger = logger;
        this.notificationService = notificationService;
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(OperatorAuthenticatedEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.AuthenticationSuccessfull(notification.Operator);

        logger.LogDebug("OperatorAuthenticatedEvent Handled");
    }
}