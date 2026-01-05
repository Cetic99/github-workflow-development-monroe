using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using MediatR;

namespace CashVault.Application.Features.CreditFeatures.Queries;

public record GetPosibleBillsToDispenseQuery : IRequest<PosibleBillsToDispenseDto>
{
}

internal sealed class GetPosibleBillsToDispenseQueryHandler : IRequestHandler<GetPosibleBillsToDispenseQuery, PosibleBillsToDispenseDto>
{
    private readonly ITerminal _terminal;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMoneyStatusRepository _moneyStatusRepository;

    public GetPosibleBillsToDispenseQueryHandler(ITerminal terminal, IUnitOfWork unitOfWork, IMoneyStatusRepository moneyStatusRepository)
    {
        _terminal = terminal;
        _unitOfWork = unitOfWork;
        _moneyStatusRepository = moneyStatusRepository;
    }

    public async Task<PosibleBillsToDispenseDto> Handle(GetPosibleBillsToDispenseQuery request, CancellationToken cancellationToken)
    {
        if (_terminal?.OperatingMode == TerminalOperatingMode.Operator)
        {
            throw new InvalidOperationException("Dispensing is not allowed in operator mode");
        }

        var creditStatus = await _moneyStatusRepository.GetCurrentCreditStatusAsync();

        decimal amountHint = creditStatus.Amount;

        var payoutRules = await _unitOfWork.TerminalRepository.GetPayoutRulesConfigurationAsync();

        var billStatus = await _unitOfWork.MoneyStatusRepository.GetDispenserBillCountStatusAsync();

        if (billStatus.Cassettes.Any(billStatus => billStatus.CurrentBillCount > 0))
        {
            List<DenominationDispenseOption> dispenseOptions = billStatus.GetDenominationDispenseOptions(amountHint);

            var prefilledCombinations = billStatus.GetPrefilledCombinationsForAmount(amountHint);

            var prefilledAmountToPrint = creditStatus.Amount - prefilledCombinations.Sum(x => x.Count * x.Denomination);

            return new PosibleBillsToDispenseDto(payoutRules.Bills.Min, payoutRules.Bills.Max, creditStatus.Amount, creditStatus.Currency, dispenseOptions, prefilledCombinations, prefilledAmountToPrint, _terminal.AmountPrecision);
        }
        else
        {
            return new PosibleBillsToDispenseDto(payoutRules.Bills.Min, payoutRules.Bills.Max, creditStatus.Amount, creditStatus.Currency, null, null, creditStatus.Amount, _terminal.AmountPrecision);
        }
    }
}
