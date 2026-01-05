using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.EventHandlers;

internal sealed class UserWidgetsUpdatedEventHandler(INotificationService notificationService, IUnitOfWork unitOfWork)
    : INotificationHandler<UserWidgetsUpdatedEvent>
{
    public async Task Handle(UserWidgetsUpdatedEvent notification, CancellationToken cancellationToken)
    {
        unitOfWork.TerminalRepository.UpdateUserWidgetsConfiguration(notification.UserWidgetsConfiguration);
        await unitOfWork.SaveChangesAsync();

        List<UserWidget> enabledWidgets = [.. notification.UserWidgetsConfiguration
            .Widgets.Where(w => w.Enabled)
            .OrderBy(w => w.DisplaySequence)];

        await notificationService.UserWidgetsUpdated(enabledWidgets);
    }
}
