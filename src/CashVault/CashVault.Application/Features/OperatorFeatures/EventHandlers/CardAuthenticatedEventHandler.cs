using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.OperatorFeatures.EventHandlers;

internal sealed class CardAuthenticatedEventHandler : INotificationHandler<CardAuthenticatedEvent>
{
    private readonly ILogger<CardAuthenticatedEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public CardAuthenticatedEventHandler(ILogger<CardAuthenticatedEventHandler> logger,
                                         IUnitOfWork unitOfWork,
                                         INotificationService notificationService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task Handle(CardAuthenticatedEvent notification, CancellationToken cancellationToken)
    {
        var cardUuid = notification.CardUuid;
        var cardUID = notification.CardUID;

        if (cardUuid.Equals(Guid.Empty))
        {
            _logger.LogWarning("CardAuthenticatedEvent received with empty Uuid.");
            return;
        }

        var operatorCard = await _unitOfWork.OperatorRepository.GetIdentificationCardByUuidAndUIDAsync(cardUuid, cardUID);

        if (operatorCard == null)
        {
            _logger.LogWarning($"No operator found with card uuid: {cardUuid}");
            return;
        }

        try
        {
            var @operator = await _unitOfWork.OperatorRepository.GetOperatorWithIdentificationCards(operatorCard.OperatorId);

            if (@operator == null)
            {
                _logger.LogError($"No operator found with ID: {operatorCard.OperatorId}");
                return;
            }

            @operator.AuthenticateWithIdentificationCard(cardUuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling CardAuthenticatedEvent for card uuid: {cardUuid}", cardUuid);
            await _notificationService.AuthenicationFailed();
        }
        finally
        {
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
