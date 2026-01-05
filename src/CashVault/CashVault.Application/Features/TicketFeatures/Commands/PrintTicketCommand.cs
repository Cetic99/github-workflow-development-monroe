using CashVault.Application.Common;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.TicketFeatures.Commands;

public record PrintTicketCommand : ISynchronizationCommand, IRequest<bool>
{
    public Guid requestId { get; init; }
    public decimal AmountRequested { get; init; }
}

public class PrintTicketCommandHandler : IRequestHandler<PrintTicketCommand, bool>
{
    private readonly ILogger<PrintTicketCommandHandler> logger;
    private readonly ITerminal terminal;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICMSService cmsService;
    private readonly IRegionalService regionalService;

    public PrintTicketCommandHandler(ILogger<PrintTicketCommandHandler> logger,
        IRegionalService regionalService,
        IUnitOfWork unitOfWork,
        ITerminal terminal,
        ICMSService cmsService)
    {
        this.logger = logger;
        this.terminal = terminal;
        this.unitOfWork = unitOfWork;
        this.cmsService = cmsService;
        this.regionalService = regionalService;
    }

    public async Task<bool> Handle(PrintTicketCommand request, CancellationToken cancellationToken)
    {
        bool result = false;
        var amountToPrint = BaseHelper.RoundNumber(request.AmountRequested, terminal.AmountPrecision);

        logger.LogDebug("Ticket printing requested");

        if (terminal?.OperatingMode == TerminalOperatingMode.Operator)
        {
            // TODO: dispatch fail event
            throw new InvalidOperationException("Printing is not allowed in operator mode");
        }

        if (terminal?.TITOPrinter == null)
        {
            // TODO: dispatch fail event
            throw new InvalidOperationException("Ticket printing is not avaliable");
        }

        var payoutRules = await unitOfWork.TerminalRepository.GetPayoutRulesConfigurationAsync();
        if (amountToPrint < payoutRules.Tickets.Min || amountToPrint > payoutRules.Tickets.Max)
        {
            // TODO: dispatch fail event
            throw new InvalidOperationException("Requested ticket amount is out of allowed range.");
        }

        var creditStatus = await unitOfWork.MoneyStatusRepository.GetCurrentCreditStatusAsync();
        decimal previousCreditAmount = creditStatus.Amount;
        if (creditStatus.Amount < amountToPrint)
        {
            throw new InvalidOperationException("Amount requested is greater than credit available.");
        }


        var onlineIntegrations = await unitOfWork.TerminalRepository.GetCurrentOnlineIntegrationsConfigurationAsync();

        DateTime printRequestedAt = regionalService.ConvertToUserTimeZone(DateTime.UtcNow, terminal.LocalTimeZone);

        if (onlineIntegrations.CasinoManagementSystem)
        {
            logger.LogDebug("CMS ticket requested");

            var cmsResponse = await cmsService.RequestTicketPrinting(amountToPrint);
            bool isLocalTicket = false;

            if (cmsResponse == null || !cmsResponse.IsSuccessful)
            {
                logger.LogError("CMS system is not available");
            }
            else
            {
                var currency = Currency.Default;

                var ticket = new Ticket(TicketType.TITO, cmsResponse.Barcode, isLocalTicket, "0000", amountToPrint, currency: currency, daysValid: cmsResponse.Validity);
                ticket.RequestPrinting(printRequestedAt);
                await unitOfWork.TicketRepository.SaveTicketAsync(ticket);

                var transaction = new TicketTransaction(ticket, TransactionType.Debit, previousCreditAmount, TicketTypeDetail.Cms, $"CMS Ticket printed with barcode {ticket.Barcode}");
                await unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);

                OperationResult printingResult = await terminal.TITOPrinter.PrintTicketAsync(ticket, caption: cmsResponse.Location, locationAddress: cmsResponse.Address2, locationName: cmsResponse.Address1, machineNumber: onlineIntegrations.DeviceId);

                if (!printingResult.IsSuccess)
                {
                    logger.LogError($"Failed to print ticket {ticket.Barcode}. {printingResult.ErrorMessage}");
                    await cmsService.FailTicketPrinting(ticket.Barcode);
                    transaction.FailTransaction($"Failed to print ticket. {printingResult.ErrorMessage}", newCreditAmount: creditStatus.Amount);
                    await unitOfWork.SaveChangesAsync();
                    result = false;
                }
                else
                {
                    bool completitionResponse = await cmsService.CompleteTicketPrinting(ticket.Barcode);
                    if (!completitionResponse)
                    {
                        logger.LogError($"Failed to complete ticket printing {ticket.Barcode}. CMS did not respond with success.");
                        transaction.FailTransaction($"Printing failed. CMS did not respond with success.", creditStatus.Amount);
                        await unitOfWork.SaveChangesAsync();
                        result = false;
                    }
                    else
                    {


                        creditStatus.DecreaseAmount(ticket.Amount);

                        ticket.CompletePrinting();
                        transaction.CompleteTransaction(newCreditAmount: creditStatus.Amount);
                        unitOfWork.MoneyStatusRepository.UpdateCurrentCreditStatus(creditStatus);

                        await unitOfWork.SaveChangesAsync();

                        result = true;
                    }
                }

            }
        }

        if (result == false)
        {
            //print local ticket if CMS is not available or failed
            result = await PrintLocalTicket(amountToPrint, printRequestedAt);
        }

        return result;
    }

    private async Task<bool> PrintLocalTicket(decimal amountRequested, DateTime printRequestedAt)
    {
        bool isLocalTicket = true;
        var creditStatus = await unitOfWork.MoneyStatusRepository.GetCurrentCreditStatusAsync();
        decimal previousCreditAmount = creditStatus.Amount;

        logger.LogDebug("Local ticket requested");

        string barcodeCandidate = GetBarcodeCandidate();
        int i = 0;
        while (unitOfWork.TicketRepository.DoesBarcodeExist(barcodeCandidate) && i < 20)
        {
            barcodeCandidate = GetBarcodeCandidate();
            i++;
        }

        if (i == 20)
        {
            logger.LogError($"Failed to generate unique barcode after {i + 1} retries.");
            return false;
        }

        var currency = Currency.Default;
        var ticket = new Ticket(TicketType.TITO, barcodeCandidate, isLocalTicket, "0000", amountRequested, currency: currency);
        ticket.RequestPrinting(printRequestedAt);

        var transaction = new TicketTransaction(ticket, TransactionType.Debit, previousCreditAmount, TicketTypeDetail.Local, $"Ticket printed with barcode {ticket.Barcode}");

        await unitOfWork.TicketRepository.SaveTicketAsync(ticket);
        await unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);

        var config = await unitOfWork.TerminalRepository.GetCurrentRegionalConfigurationAsync();

        OperationResult printingResult = await terminal.TITOPrinter.PrintTicketAsync(ticket, $"LOCAL TICKET - {config.Caption}", config.LocationName, config.LocationAddress, config.MachineName);

        if (!printingResult.IsSuccess)
        {
            logger.LogError($"Failed to print ticket {ticket.Barcode}. {printingResult.ErrorMessage}");
            transaction.FailTransaction($"Failed to print ticket {ticket.Barcode}. {printingResult.ErrorMessage}", creditStatus.Amount);
            await unitOfWork.SaveChangesAsync();
            return false;
        }

        creditStatus.DecreaseAmount(ticket.Amount);

        ticket.CompletePrinting();
        transaction.CompleteTransaction(newCreditAmount: creditStatus.Amount);

        unitOfWork.MoneyStatusRepository.UpdateCurrentCreditStatus(creditStatus);

        await unitOfWork.SaveChangesAsync();

        return printingResult.IsSuccess;
    }

    private string GetBarcodeCandidate()
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Random random = new Random();
        string randomDigits = random.Next(10000, 99999).ToString();

        string barcode = "01" + timestamp.ToString().Substring(0, 11) + randomDigits;
        return barcode;
    }
}

public class PrintTicketCommandValidator : AbstractValidator<PrintTicketCommand>
{
    public PrintTicketCommandValidator()
    {
    }
}