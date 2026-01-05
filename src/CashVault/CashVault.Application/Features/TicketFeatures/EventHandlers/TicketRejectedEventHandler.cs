﻿using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.TicketFeatures.EventHandlers;

public class TicketRejectedEventHandler : INotificationHandler<TicketRejectedEvent>
{
    private readonly ILogger<TicketRejectedEventHandler> logger;
    private readonly INotificationService notificationService;

    public TicketRejectedEventHandler(ILogger<TicketRejectedEventHandler> logger, INotificationService notificationService)
    {
        this.logger = logger;
        this.notificationService = notificationService;
    }

    public async Task Handle(TicketRejectedEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.TicketRejected();

        logger.LogDebug("TicketRejectedEvent handled");
    }
}
