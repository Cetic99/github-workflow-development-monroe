using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands
{
    public class EmptyBillDispenserCassettesCommand : IRequest<Unit>
    {
        public List<int> CassettesNumbers { get; set; } = [];
        public bool EmptyRejectBin { get; set; } = false;
    }

    internal sealed class EmptyBillDispenserCassettesCommandHandler : IRequestHandler<EmptyBillDispenserCassettesCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionService _sessionService;

        public EmptyBillDispenserCassettesCommandHandler(IUnitOfWork unitOfWork, ISessionService sessionService)
        {
            _unitOfWork = unitOfWork;
            _sessionService = sessionService;
        }

        public async Task<Unit> Handle(EmptyBillDispenserCassettesCommand command, CancellationToken cancellationToken)
        {
            DispenserBillCountStatus dispenserMoneyStatus = await _unitOfWork.MoneyStatusRepository.GetDispenserBillCountStatusAsync();
            DispenserBillCountStatus oldDispenserMoneyStatus = (DispenserBillCountStatus)dispenserMoneyStatus.Clone();
            BillTicketAcceptorStackerStatus acceptorMoneyStatus = await _unitOfWork.MoneyStatusRepository.GetBillTicketAcceptorBillCountStatusAsync();
            BillTicketAcceptorStackerStatus oldAcceptorMoneyStatus = (BillTicketAcceptorStackerStatus)acceptorMoneyStatus.Clone();

            foreach (int cassetteNumber in command.CassettesNumbers)
            {
                dispenserMoneyStatus.EmptyCassette(cassetteNumber);
            }

            var moneyStatusTransaction = new MoneyStatusTransaction(
                oldAcceptorMoneyStatus,
                acceptorMoneyStatus,
                oldDispenserMoneyStatus,
                dispenserMoneyStatus,
                DeviceType.BillDispenser,
                _sessionService?.User?.Email,
                _sessionService?.User?.Username);

            // If there is no change in the dispenser status, do not add the transaction
            if (moneyStatusTransaction.Amount != 0)
            {
                _unitOfWork.MoneyStatusRepository.AddMoneyStatusTransaction(moneyStatusTransaction);
            }

            if (command.EmptyRejectBin)
            {
                dispenserMoneyStatus.EmptyRejectBin();
            }

            _unitOfWork.MoneyStatusRepository.UpdateDispenserBillCountStatus(dispenserMoneyStatus);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }

    public class EmptyBillDispenserCassettesCommandCommandValidator : AbstractValidator<EmptyBillDispenserCassettesCommand>
    {
        public EmptyBillDispenserCassettesCommandCommandValidator(ILocalizer t)
        {
            RuleFor(x => x.CassettesNumbers).NotNull()
                .Must(x => x.Count > 0)
                .When(x => !x.EmptyRejectBin)
                    .WithMessage(t["At least one cassette number must be provided"])
                .Must(x => x.Count == x.Distinct().Count())
                    .WithMessage(t["Cassette numbers must be unique"]);
        }
    }
}
