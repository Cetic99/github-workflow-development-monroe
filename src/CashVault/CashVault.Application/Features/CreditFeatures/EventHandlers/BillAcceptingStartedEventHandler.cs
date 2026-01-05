using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.TransactionAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.CreditFeatures.EventHandlers;

public class BillAcceptingStartedEventHandler : INotificationHandler<BillAcceptingStartedEvent>
{
    private readonly ILogger<BillAcceptingStartedEventHandler> logger;
    private readonly INotificationService notificationService;
    private readonly IUnitOfWork unitOfWork;

    public BillAcceptingStartedEventHandler(ILogger<BillAcceptingStartedEventHandler> logger, INotificationService notificationService, IUnitOfWork unitOfWork)
    {
        this.logger = logger;
        this.notificationService = notificationService;
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(BillAcceptingStartedEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.BillAccepting();

        logger.LogDebug("BillAcceptingStartedEvent Handled");
    }
}
