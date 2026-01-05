using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.EventHandlers
{
    internal sealed class DeviceDeactivatedEventHandler : INotificationHandler<DeviceDeactivatedEvent>
    {
        private readonly INotificationService _notificationService;

        public DeviceDeactivatedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(DeviceDeactivatedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.DeviceDeactivated(notification);
        }
    }
}
