using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.Aggregates.TransactionAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.CreditFeatures.EventHandlers;

public class CoinAcceptedEventHandler : INotificationHandler<CoinAcceptedEvent>
{
    private readonly ILogger<CoinAcceptedEventHandler> logger;
    private readonly INotificationService notificationService;
    private readonly IUnitOfWork unitOfWork;

    public CoinAcceptedEventHandler(
        ILogger<CoinAcceptedEventHandler> logger,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        this.logger = logger;
        this.notificationService = notificationService;
        this.unitOfWork = unitOfWork;
    }


    public async Task Handle(CoinAcceptedEvent notification, CancellationToken cancellationToken)
    {
        var currentCreditStatus = await unitOfWork.MoneyStatusRepository.GetCurrentCreditStatusAsync();
        decimal previousCreditAmount = currentCreditStatus.Amount;

        var transaction = new AcceptorCoinTransaction(
            notification.Amount,
            previousCreditAmount);

        await unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);

        var acceptorCoinStatus = await unitOfWork.MoneyStatusRepository.GetCoinAcceptorCollectorStatusAsync();
        acceptorCoinStatus.AddCoin(1, notification.Amount);
        unitOfWork.MoneyStatusRepository.UpdateCoinAcceptorCollectorStatus(acceptorCoinStatus);

        currentCreditStatus.IncreaseAmount(notification.Amount);
        unitOfWork.MoneyStatusRepository.UpdateCurrentCreditStatus(currentCreditStatus);

        transaction.CompleteTransaction(newCreditAmount: currentCreditStatus.Amount);

        await unitOfWork.SaveChangesAsync();

        await notificationService.CoinAccepted(notification.Amount, notification.Currency);

        logger.LogDebug("CoinAcceptedEvent Handled");
    }
}
