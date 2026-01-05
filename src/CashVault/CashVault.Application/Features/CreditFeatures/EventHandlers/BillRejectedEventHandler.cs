﻿using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.CreditFeatures.EventHandlers;

public class BillRejectedEventHandler : INotificationHandler<BillRejectedEvent>
{
    private readonly ILogger<BillRejectedEventHandler> logger;
    private readonly INotificationService notificationService;

    public BillRejectedEventHandler(ILogger<BillRejectedEventHandler> logger, INotificationService notificationService)
    {
        this.logger = logger;
        this.notificationService = notificationService;
    }

    public async Task Handle(BillRejectedEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.BillRejected();

        logger.LogDebug("BillRejectedEvent Handled");
    }
}
