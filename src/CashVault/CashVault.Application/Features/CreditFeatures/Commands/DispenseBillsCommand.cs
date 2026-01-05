using CashVault.Application.Common;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Aggregates.MoneyStatusAggregate.Exceptions;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.CreditFeatures.Commands;

public record DispenseBillsCommand : ISynchronizationCommand, IRequest<decimal>
{
    public Guid RequestId { get; init; }
    public decimal AmountRequested { get; init; }
    public List<DenominationCount>? BillSpecification { get; init; }

    public DispenseBillsCommand(Guid requestId, decimal amountRequested, List<DenominationCount>? billSpecification)
    {
        RequestId = requestId;
        AmountRequested = amountRequested;
        BillSpecification = billSpecification ?? [];
    }
}

public class DispenseBillsCommandHandler : IRequestHandler<DispenseBillsCommand, decimal>
{
    private readonly ILogger<DispenseBillsCommandHandler> logger;
    private readonly ITerminal terminal;
    private readonly IUnitOfWork unitOfWork;

    public DispenseBillsCommandHandler(ILogger<DispenseBillsCommandHandler> logger, ITerminal terminal, IUnitOfWork unitOfWork)
    {
        this.logger = logger;
        this.terminal = terminal;
        this.unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(DispenseBillsCommand request, CancellationToken cancellationToken)
    {
        decimal result = 0;
        var amountToDispense = BaseHelper.RoundNumber(request.AmountRequested, terminal.AmountPrecision);

        logger.LogDebug("Bill dispensing requested");

        if (terminal?.OperatingMode == TerminalOperatingMode.Operator)
        {
            // TODO: dispatch fail event
            throw new InvalidOperationException("Dispensing is not allowed in operator mode");
        }

        var payoutRules = await unitOfWork.TerminalRepository.GetPayoutRulesConfigurationAsync();
        if (amountToDispense < payoutRules.Bills.Min || amountToDispense > payoutRules.Bills.Max)
        {
            // TODO: dispatch fail event
            throw new InvalidOperationException("Requested bill dispensing amount is out of allowed range");
        }

        var currentCreditStatus = await unitOfWork.MoneyStatusRepository.GetCurrentCreditStatusAsync();
        decimal previousCreditAmount = currentCreditStatus.Amount;

        if (currentCreditStatus.Amount < amountToDispense)
        {
            throw new InvalidOperationException("Amount requested is greater than credit available.");
        }

        var billStatus = await unitOfWork.MoneyStatusRepository.GetDispenserBillCountStatusAsync();

        List<DispenserBillTransactionItem> transactionItems;

        try
        {
            transactionItems = billStatus.CalculateDispenseSpecifications(amountToDispense, request.BillSpecification);
        }
        catch (InsufficientDispenserBillsException ex)
        {
            logger.LogError(ex, ex?.Message);
            throw;
        }

        var transaction = new DispenserBillTransaction(amountToDispense, previousCreditAmount, "Cash payout requested");
        transaction.AddItemRange(transactionItems);

        await unitOfWork.TransactionRepository.SaveTransactionAsync(transaction);

        //TODO: When we introduce operation model, error message should be included into transaction remarks.
        OperationResult dispenseResult = await terminal.BillDispenser.DispenseCashAsync(transaction);

        billStatus.UpdateCassetteBillCountFromTransaction(transaction);
        decimal amountDispensed = transaction.Items.Sum(i => i.AmountDispensed);

        currentCreditStatus.DecreaseAmount(amountDispensed);
        unitOfWork.MoneyStatusRepository.UpdateDispenserBillCountStatus(billStatus);
        unitOfWork.MoneyStatusRepository.UpdateCurrentCreditStatus(currentCreditStatus);

        if (amountDispensed == 0)
        {
            // Add reason for failure to transaction remarks
            logger.LogError("Dispensing failed. No bills dispensed.");
            transaction.FailTransaction($"Dispensing failed: \n{dispenseResult.ErrorMessage}", newCreditAmount: currentCreditStatus.Amount);
        }
        else
        {
            transaction.CompleteTransaction(amountDispensed, newCreditAmount: currentCreditStatus.Amount, dispenseResult.ErrorMessage);
        }

        await unitOfWork.SaveChangesAsync();
        result = amountDispensed;
        return result;
    }
}

public class DispenseBillsCommandValidator : AbstractValidator<DispenseBillsCommand>
{
    public DispenseBillsCommandValidator()
    {
        RuleFor(r => r.AmountRequested).GreaterThan(0);

        When(r => r.BillSpecification != null, () =>
        {
            RuleForEach(r => r.BillSpecification).ChildRules(billSpec =>
            {
                billSpec.RuleFor(b => b.Denomination).GreaterThan(0);
                billSpec.RuleFor(b => b.Count).GreaterThanOrEqualTo(0);
            });

            RuleFor(r => r.BillSpecification)
                .Must(billSpec => billSpec.GroupBy(b => b.Denomination).All(g => g.Count() == 1))
                .WithMessage("Duplicate denominations are not allowed.");

            RuleFor(r => r.BillSpecification)
                .Must((command, billSpec) => billSpec.Sum(b => b.Denomination * b.Count) <= command.AmountRequested)
                .WithMessage("The total value of the bill specification cannot exceed the amount requested.");
        });
    }
}
