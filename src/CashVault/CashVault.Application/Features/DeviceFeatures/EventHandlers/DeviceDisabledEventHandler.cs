using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.EventHandlers
{
    internal sealed class DeviceDisabledEventHandler : INotificationHandler<DeviceDisabledEvent>
    {
        private readonly INotificationService _notificationService;

        public DeviceDisabledEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(DeviceDisabledEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.DeviceDisabled(notification);
        }
    }
}
