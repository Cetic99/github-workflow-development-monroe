using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.Aggregates.TransactionAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.CreditFeatures.EventHandlers;

public class BillAcceptedEventHandler : INotificationHandler<BillAcceptedEvent>
{
    private readonly ILogger<BillAcceptedEventHandler> logger;
    private readonly INotificationService notificationService;
    private readonly IUnitOfWork unitOfWork;

    public BillAcceptedEventHandler(ILogger<BillAcceptedEventHandler> logger, INotificationService notificationService, IUnitOfWork unitOfWork)
    {
        this.logger = logger;
        this.notificationService = notificationService;
        this.unitOfWork = unitOfWork;
    }

    public async Task Handle(BillAcceptedEvent notification, CancellationToken cancellationToken)
    {
        var currentCreditStatus = await unitOfWork.MoneyStatusRepository.GetCurrentCreditStatusAsync();
        decimal previousCreditAmount = currentCreditStatus.Amount;

        var transaction = new AcceptorBillTransaction(notification.Amount, previousCreditAmount);
        await unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);

        var acceptorMoneyStatus = await unitOfWork.MoneyStatusRepository.GetBillTicketAcceptorBillCountStatusAsync();
        acceptorMoneyStatus.AddBill(1, notification.Amount);
        unitOfWork.MoneyStatusRepository.UpdateBillTicketAcceptorBillCountStatus(acceptorMoneyStatus);


        currentCreditStatus.IncreaseAmount(notification.Amount);
        unitOfWork.MoneyStatusRepository.UpdateCurrentCreditStatus(currentCreditStatus);

        transaction.CompleteTransaction(newCreditAmount: currentCreditStatus.Amount);

        await unitOfWork.SaveChangesAsync();

        await notificationService.BillAccepted(notification.Amount, notification.Currency);

        logger.LogDebug("BillAcceptedEvent Handled");
    }
}
