using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using CashVault.Domain.Aggregates.MessageAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.AdministrationFeatures.EventHandlers
{
    internal sealed class MessageUpdatedEventHandler : INotificationHandler<MessageUpdatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IRegionalService _regionalService;

        public MessageUpdatedEventHandler(INotificationService notificationService, IRegionalService regionalService)
        {
            _notificationService = notificationService;
            _regionalService = regionalService;
        }

        public async Task Handle(MessageUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var result = await _regionalService.UpdateSingleMessage
                (notification.LanguageCode, notification.Key, notification.Value);

            if (result)
                await _notificationService.MessageUpdated
                    (notification.LanguageCode, notification.Key, notification.Value);
        }
    }
}
