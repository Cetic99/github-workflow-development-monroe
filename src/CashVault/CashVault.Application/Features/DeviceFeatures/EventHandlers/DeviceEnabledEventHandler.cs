using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.EventHandlers
{
    internal sealed class DeviceEnabledEventHandler : INotificationHandler<DeviceEnabledEvent>
    {
        private readonly INotificationService _notificationService;

        public DeviceEnabledEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(DeviceEnabledEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.DeviceEnabled(notification);
        }
    }
}
