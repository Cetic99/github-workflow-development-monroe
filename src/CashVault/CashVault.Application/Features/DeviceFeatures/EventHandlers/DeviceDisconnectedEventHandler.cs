using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.EventHandlers
{
    internal sealed class DeviceDisconnectedEventHandler : INotificationHandler<DeviceDisconnectedEvent>
    {
        private readonly INotificationService _notificationService;

        public DeviceDisconnectedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(DeviceDisconnectedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.DeviceDisconnected(notification);
        }
    }
}
