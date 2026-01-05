using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.TransactionAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.TicketFeatures.Commands;

public class SendTicketRedemptionAckCommand : IRequest<bool>
{
    public string Barcode { get; init; } = null!;
    public TicketType TicketType { get; init; } = null!;
    public Guid Id { get; set; }

    public SendTicketRedemptionAckCommand(string barcode, TicketType ticketType, Guid id)
    {
        Barcode = barcode;
        TicketType = ticketType;
        Id = id;
    }
}

internal sealed class SendTicketRedemptionAckCommandHandler : IRequestHandler<SendTicketRedemptionAckCommand, bool>
{
    private readonly ILogger<SendTicketRedemptionNackCommandHandler> _logger;
    private readonly ITicketProviderFactory _ticketProviderFactory;
    private readonly IUnitOfWork _unitOfWork;

    public SendTicketRedemptionAckCommandHandler(ILogger<SendTicketRedemptionNackCommandHandler> logger,
                                                  ITicketProviderFactory ticketProviderFactory,
                                                  IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _ticketProviderFactory = ticketProviderFactory;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SendTicketRedemptionAckCommand command, CancellationToken cancellationToken)
    {
        var _betboxProvider = _ticketProviderFactory.GetTicketProviderByType(command.TicketType);
        var barcode = command.Barcode;

        try
        {
            var result = await _betboxProvider.RedeemTicketAck(barcode, command.Id);

            var ticketInfo = await _betboxProvider.RedeemTicket(barcode, command.Id);

            if (ticketInfo == null)
            {
                _logger.LogWarning($"Ticket with barcode {barcode} not found. [Provider: {command.TicketType}].");
                return false;
            }

            if (!result)
            {
                _logger.LogError($"Redeem Betbox ticket with barcode {barcode} failed.");
                return false;
            }

            var currentCreditStatus = await _unitOfWork.MoneyStatusRepository.GetCurrentCreditStatusAsync();
            var previousCreditAmount = currentCreditStatus.Amount;

            currentCreditStatus.IncreaseAmount(ticketInfo.TotalAmount);
            _unitOfWork.MoneyStatusRepository.UpdateCurrentCreditStatus(currentCreditStatus);

            var transaction = new TicketTransaction(ticketInfo.TotalAmount, TransactionType.Credit, TicketType.BetboxTicket, previousCreditAmount, externalReference: barcode, $"Received Betbox ticket with barcode: {barcode}");
            transaction.CompleteTransaction(newCreditAmount: currentCreditStatus.Amount);

            await _unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);

            await _unitOfWork.SaveChangesAsync();



            _logger.LogInformation($"Betbox ticket with barcode {barcode} redeemed.");
            return true;
        }
        catch (Exception ex)
        {
            await _betboxProvider.RedeemTicketNack(barcode, command.Id);

            _logger.LogError(ex, $"Failed to redeem betbox ticket with barcode: [{command.Barcode}]");
            throw;
        }
    }
}
