using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.EventHandlers
{
    internal sealed class DeviceConnectedEventHandler : INotificationHandler<DeviceConnectedEvent>
    {
        private readonly INotificationService _notificationService;
        public DeviceConnectedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public async Task Handle(DeviceConnectedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.DeviceConnected(notification);
        }
    }
}
