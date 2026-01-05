using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.EventHandlers
{
    internal sealed class DeviceActivatedEventHandler : INotificationHandler<DeviceActivatedEvent>
    {
        private readonly INotificationService _notificationService;
        public DeviceActivatedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public async Task Handle(DeviceActivatedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.DeviceActivated(notification);
        }
    }
}
