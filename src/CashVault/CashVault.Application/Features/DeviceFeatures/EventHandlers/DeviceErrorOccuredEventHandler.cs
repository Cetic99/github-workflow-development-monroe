using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.EventHandlers
{
    internal sealed class DeviceErrorOccuredEventHandler : INotificationHandler<DeviceErrorOccuredEvent>
    {
        private readonly INotificationService _notificationService;

        public DeviceErrorOccuredEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(DeviceErrorOccuredEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.DeviceError(notification);
        }
    }
}
