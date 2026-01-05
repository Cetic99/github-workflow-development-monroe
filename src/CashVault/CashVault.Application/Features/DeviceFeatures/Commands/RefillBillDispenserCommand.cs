using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class RefillBillDispenserCommand : IRequest<Unit>
{
    public class RefillBillCassette
    {
        public int CassetteNumber { get; set; }
        public int BillCount { get; set; }
    }
    public List<RefillBillCassette> Cassettes { get; set; } = [];
}

public class RefillBillDispenserCommandCommandHandler : IRequestHandler<RefillBillDispenserCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionService _sessionService;

    public RefillBillDispenserCommandCommandHandler(IUnitOfWork unitOfWork, ISessionService sessionService)
    {
        _unitOfWork = unitOfWork;
        _sessionService = sessionService;
    }

    public async Task<Unit> Handle(RefillBillDispenserCommand command, CancellationToken cancellationToken)
    {
        DispenserBillCountStatus dispenserMoneyStatus = await _unitOfWork.MoneyStatusRepository.GetDispenserBillCountStatusAsync();
        DispenserBillCountStatus oldDispenserMoneyStatus = (DispenserBillCountStatus)dispenserMoneyStatus.Clone();
        BillTicketAcceptorStackerStatus acceptorMoneyStatus = await _unitOfWork.MoneyStatusRepository.GetBillTicketAcceptorBillCountStatusAsync();
        BillTicketAcceptorStackerStatus oldAcceptorMoneyStatus = (BillTicketAcceptorStackerStatus)acceptorMoneyStatus.Clone();


        foreach (var cassette in command.Cassettes.Where(c => c.BillCount > 0))
        {
            dispenserMoneyStatus.RefillCassette(cassette.CassetteNumber, cassette.BillCount);
        }

        var moneyStatusTransaction = new MoneyStatusTransaction(oldAcceptorMoneyStatus,
                                                                acceptorMoneyStatus,
                                                                oldDispenserMoneyStatus,
                                                                dispenserMoneyStatus,
                                                                DeviceType.BillDispenser,
                                                                _sessionService?.User?.Email,
                                                                _sessionService?.User?.Username);

        _unitOfWork.MoneyStatusRepository.AddMoneyStatusTransaction(moneyStatusTransaction);

        _unitOfWork.MoneyStatusRepository.UpdateDispenserBillCountStatus(dispenserMoneyStatus);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}

public class RefillBillDispenserCommandValidator : AbstractValidator<RefillBillDispenserCommand>
{
    public RefillBillDispenserCommandValidator(ILocalizer t)
    {
        RuleFor(x => x.Cassettes).NotNull()
            .Must(x => x.Count > 0).WithMessage(t["At least one cassette must be configured"])
            .Must(x => x.Any(c => c.BillCount > 0)).WithMessage(t["At least one bill must be added to a cassette"]);
        RuleFor(x => x.Cassettes).Must(x => x.Count == x.Select(c => c.CassetteNumber).Distinct().Count())
            .WithMessage(t["Cassette numbers must be unique"]);
    }
}
