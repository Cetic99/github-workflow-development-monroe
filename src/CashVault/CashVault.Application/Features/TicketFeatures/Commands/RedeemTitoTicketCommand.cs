using CashVault.Application.Common;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.TransactionAggregate;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.TicketFeatures.Commands;

public record RedeemTitoTicketCommand : ISynchronizationCommand, IRequest<bool>
{
    public string Barcode { get; init; }

    public RedeemTitoTicketCommand(string barcode)
    {
        Barcode = barcode;
    }
}

public class RedeemTitoTicketCommandHandler : IRequestHandler<RedeemTitoTicketCommand, bool>
{
    private readonly ILogger<RedeemTitoTicketCommandHandler> logger;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICMSService _cmsService;
    private readonly ITerminal terminal;

    public RedeemTitoTicketCommandHandler(
        ILogger<RedeemTitoTicketCommandHandler> logger,
        IUnitOfWork unitOfWork,
        ICMSService cmsService,
        ITerminal terminal)
    {
        this.logger = logger;
        this.unitOfWork = unitOfWork;
        _cmsService = cmsService;
        this.terminal = terminal;
    }

    public async Task<bool> Handle(RedeemTitoTicketCommand command, CancellationToken cancellationToken)
    {
        bool result = false;
        var creditStatus = await unitOfWork.MoneyStatusRepository.GetCurrentCreditStatusAsync();
        decimal previousCreditAmount = creditStatus.Amount;

        if (terminal?.OperatingMode == TerminalOperatingMode.Operator)
        {
            logger.LogError("Can't redeem ticket, terminal in operator mode");
            throw new InvalidOperationException("Redeeming is not allowed in operator mode");
        }

        logger.LogDebug("Redeem ticket requested");

        var onlineIntegrations = await unitOfWork.TerminalRepository.GetCurrentOnlineIntegrationsConfigurationAsync();

        if (onlineIntegrations.CasinoManagementSystem)
        {
            bool redeemed = await RedeemRemoteTicket(command.Barcode, previousCreditAmount);

            if (redeemed)
            {
                return true;
            }
        }

        result = await RedeemLocalTicket(command.Barcode, previousCreditAmount);

        return result;
    }

    private async Task<bool> RedeemLocalTicket(string barcode, decimal previousCreditAmount)
    {
        Ticket ticket = await unitOfWork.TicketRepository.GetTicketByBarcodeAsync(barcode);

        if (ticket == null || ticket.IsLocal == false)
        {
            logger.LogError($"Local ticket with barcode {barcode} not found.");
            return false;
        }

        if (ticket.Redeem() == false)
        {
            logger.LogError($"Local ticket with barcode {barcode} could not be redeemed.");
            return false;
        }

        unitOfWork.TicketRepository.SaveTicketAsync(ticket);

        var transaction = new TicketTransaction(ticket, TransactionType.Credit, previousCreditAmount, TicketTypeDetail.Local, $"Redeemed local ticket with barcode: {ticket.Barcode}");

        unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation($"Local ticket with barcode {barcode} redeemed.");

        return true;
    }

    private async Task<bool> RedeemRemoteTicket(string barcode, decimal previousCreditAmount)
    {
        var redeemOnlineResponse = await _cmsService.RedeemTicket(barcode);

        if (redeemOnlineResponse == null || !redeemOnlineResponse.IsSuccessful)
        {
            logger.LogError($"Ticket with barcode {barcode} CMS validation failed.");
            return false;
        }

        var transaction = new TicketTransaction(redeemOnlineResponse.AmountWithTaxes, TransactionType.Credit, TicketType.TITO, previousCreditAmount, externalReference: barcode, $"Received CMS ticket with barcode: {barcode}", ticketTypeDetail: TicketTypeDetail.Cms);

        unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);

        await unitOfWork.SaveChangesAsync();

        var redeemAckResponse = await _cmsService.CompleteRedeemTicket(barcode);

        if (!redeemAckResponse)
        {
            logger.LogError($"Failed to acknowledge ticket with barcode {barcode} on CMS.");
            return false;
        }

        logger.LogInformation($"Ticket with barcode {barcode} redeemed on CMS.");

        return true;
    }
}

public class RedeemTitoTicketCommandValidator : AbstractValidator<RedeemTitoTicketCommand>
{
    public RedeemTitoTicketCommandValidator()
    {
        RuleFor(request => request.Barcode).NotNull().NotEmpty();
    }
}