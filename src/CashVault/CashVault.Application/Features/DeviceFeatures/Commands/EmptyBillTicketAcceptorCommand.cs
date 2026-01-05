using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands
{
    public record EmptyBillTicketAcceptorCommand : IRequest<Unit> { }

    internal sealed class EmptyBillTicketAcceptorCommandHandler : IRequestHandler<EmptyBillTicketAcceptorCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionService _sessionService;

        public EmptyBillTicketAcceptorCommandHandler(IUnitOfWork unitOfWork, ISessionService sessionService)
        {
            _unitOfWork = unitOfWork;
            _sessionService = sessionService;
        }

        public async Task<Unit> Handle(EmptyBillTicketAcceptorCommand command, CancellationToken cancellationToken)
        {
            BillTicketAcceptorStackerStatus acceptorMoneyStatus = await _unitOfWork.MoneyStatusRepository.GetBillTicketAcceptorBillCountStatusAsync();
            BillTicketAcceptorStackerStatus oldAcceptorMoneyStatus = (BillTicketAcceptorStackerStatus)acceptorMoneyStatus.Clone();
            DispenserBillCountStatus dispenserMoneyStatus = await _unitOfWork.MoneyStatusRepository.GetDispenserBillCountStatusAsync();
            DispenserBillCountStatus oldDispenserMoneyStatus = (DispenserBillCountStatus)dispenserMoneyStatus.Clone();

            acceptorMoneyStatus.Empty();

            var moneyStatusTransaction = new MoneyStatusTransaction(oldAcceptorMoneyStatus,
                                                                    acceptorMoneyStatus,
                                                                    oldDispenserMoneyStatus,
                                                                    dispenserMoneyStatus,
                                                                    DeviceType.BillAcceptor,
                                                                    _sessionService?.User?.Email,
                                                                    _sessionService?.User?.Username);

            // If there is no change in the acceptor status, do not add the transaction
            if (moneyStatusTransaction.Amount != 0)
            {
                _unitOfWork.MoneyStatusRepository.AddMoneyStatusTransaction(moneyStatusTransaction);
            }

            _unitOfWork.MoneyStatusRepository.UpdateBillTicketAcceptorBillCountStatus(acceptorMoneyStatus);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
