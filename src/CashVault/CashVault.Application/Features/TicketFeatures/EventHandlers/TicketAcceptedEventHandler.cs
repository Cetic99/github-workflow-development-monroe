using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.Aggregates.TransactionAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.TicketFeatures.EventHandlers;

public class TicketAcceptedEventHandler : INotificationHandler<TicketAcceptedEvent>
{
    private readonly ILogger<TicketAcceptedEventHandler> logger;
    private readonly INotificationService notificationService;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICMSService _CMSService;

    public TicketAcceptedEventHandler(ILogger<TicketAcceptedEventHandler> logger, INotificationService notificationService, IUnitOfWork unitOfWork, ICMSService cMSService)
    {
        this.logger = logger;
        this.notificationService = notificationService;
        this.unitOfWork = unitOfWork;
        this._CMSService = cMSService;
    }

    public async Task Handle(TicketAcceptedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Ticket accepted started handling...");

        var onlineIntegrations = await unitOfWork.TerminalRepository.GetCurrentOnlineIntegrationsConfigurationAsync();

        if (onlineIntegrations.CasinoManagementSystem)
        {
            await AcceptRemoteTicket(notification.Barcode);
        }
        else
        {
            await AcceptLocalTicket(notification.Barcode);
        }

        logger.LogDebug("TicketAcceptedEvent handled");
    }

    private async Task AcceptLocalTicket(string barcode)
    {
        Ticket ticket = await unitOfWork.TicketRepository.GetTicketByBarcodeAsync(barcode);
        if (ticket == null)
        {
            logger.LogError($"Ticket with barcode {barcode} not found");
            return;
        }

        ticket.Stack();
        await unitOfWork.TicketRepository.SaveTicketAsync(ticket);

        var transaction = await unitOfWork.TransactionRepository.GetPendingTicketTransactionAsync(ticket.Guid);
        if (transaction == null)
        {
            logger.LogError($"Transaction for ticket with barcode {barcode} not found. Searched for Guid {ticket.Guid}");
            return;
        }

        var acceptorMoneyStatus = await unitOfWork.MoneyStatusRepository.GetBillTicketAcceptorBillCountStatusAsync();
        acceptorMoneyStatus.AddTicket(ticket);
        unitOfWork.MoneyStatusRepository.UpdateBillTicketAcceptorBillCountStatus(acceptorMoneyStatus);

        var currentCreditStatus = await unitOfWork.MoneyStatusRepository.GetCurrentCreditStatusAsync();
        currentCreditStatus.IncreaseAmount(ticket.Amount);
        unitOfWork.MoneyStatusRepository.UpdateCurrentCreditStatus(currentCreditStatus);

        transaction.CompleteTransaction(newCreditAmount: currentCreditStatus.Amount);
        await unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);

        await unitOfWork.SaveChangesAsync();

        await notificationService.TicketAccepted(ticket.Amount, ticket.Currency);
    }

    private async Task AcceptRemoteTicket(string barcode)
    {
        var transaction = await unitOfWork.TransactionRepository.GetPendingTicketTransactionByExternalReference(barcode);

        if (transaction == null)
        {
            logger.LogError($"Transaction for ticket with barcode {barcode} not found on CMS. Searched for barcode: {barcode}");
            return;
        }

        var acceptorMoneyStatus = await unitOfWork.MoneyStatusRepository.GetBillTicketAcceptorBillCountStatusAsync();
        acceptorMoneyStatus.AddTicket(transaction.AmountRequested);
        unitOfWork.MoneyStatusRepository.UpdateBillTicketAcceptorBillCountStatus(acceptorMoneyStatus);

        var currentCreditStatus = await unitOfWork.MoneyStatusRepository.GetCurrentCreditStatusAsync();
        currentCreditStatus.IncreaseAmount(transaction.AmountRequested);
        unitOfWork.MoneyStatusRepository.UpdateCurrentCreditStatus(currentCreditStatus);

        transaction.CompleteTransaction(newCreditAmount: currentCreditStatus.Amount);
        await unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);

        await unitOfWork.SaveChangesAsync();

        await notificationService.TicketAccepted(transaction.AmountRequested, transaction.Currency);
    }
}
