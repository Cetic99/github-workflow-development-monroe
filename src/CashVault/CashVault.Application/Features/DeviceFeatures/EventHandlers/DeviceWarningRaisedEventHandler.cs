using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.EventHandlers
{
    internal sealed class DeviceWarningRaisedEventHandler : INotificationHandler<DeviceWarningRaisedEvent>
    {
        private readonly INotificationService _notificationService;

        public DeviceWarningRaisedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(DeviceWarningRaisedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.DeviceWarning(notification);
        }
    }
}
