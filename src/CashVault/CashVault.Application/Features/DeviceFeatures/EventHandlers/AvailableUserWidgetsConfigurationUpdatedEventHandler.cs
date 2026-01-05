using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.EventHandlers;

internal sealed class AvailableUserWidgetsConfigurationUpdatedEventHandler : INotificationHandler<AvailableUserWidgetsConfigurationUpdatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private INotificationService _notificationService;

    public AvailableUserWidgetsConfigurationUpdatedEventHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task Handle(AvailableUserWidgetsConfigurationUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _unitOfWork.TerminalRepository.UpdateAvailableUserWidgetsConfiguration(notification.AvailableUserWidgets);

        var userWidgetsConfig = await _unitOfWork.TerminalRepository.GetUserWidgetsConfigurationAsync();

        //Add new user widget if not exist with default configuration.
        foreach (var availableUserWidget in notification.AvailableUserWidgets.AvailableWidgets)
        {
            if (!userWidgetsConfig.Widgets.Any(x => x.Code.Equals(availableUserWidget.Code)))
            {
                var newWidgetConf = UserWidget.Default(availableUserWidget.Code);

                userWidgetsConfig.AddWidget(newWidgetConf);
            }
        }

        //Remove user widget from if not exist in available widgets
        var availableCodes = notification.AvailableUserWidgets.AvailableWidgets
            .Select(w => w.Code)
            .ToHashSet();

        foreach (var code in availableCodes)
        {
            if (!userWidgetsConfig.Widgets.Any(x => x.Code.Equals(code)))
            {
                userWidgetsConfig.RemoveWidget(code);
            }
        }

        _unitOfWork.TerminalRepository.UpdateUserWidgetsConfiguration(userWidgetsConfig);

        await _unitOfWork.SaveChangesAsync();

        await _notificationService.UserWidgetsUpdated(userWidgetsConfig.Widgets);
    }
}
