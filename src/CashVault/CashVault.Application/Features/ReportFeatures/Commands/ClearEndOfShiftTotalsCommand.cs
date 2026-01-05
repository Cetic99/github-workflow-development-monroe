using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using MediatR;

namespace CashVault.Application.Features.ReportFeatures.Commands;

public class ClearEndOfShiftTotalsCommand : IRequest<Unit> { }

public class ClearEndOfShiftTotalsCommandHandler(IUnitOfWork unitOfWork, ISessionService sessionService)
    : IRequestHandler<ClearEndOfShiftTotalsCommand, Unit>
{
    public async Task<Unit> Handle(ClearEndOfShiftTotalsCommand command, CancellationToken cancellationToken)
    {
        var @operator = await unitOfWork.OperatorRepository.GetById<Operator>(sessionService.User.Id);

        if (@operator == null) throw new ArgumentNullException("Operator", "Operator not found");

        @operator.HarvestShiftMoney();
        unitOfWork.SaveChanges();

        return Unit.Value;
    }
}