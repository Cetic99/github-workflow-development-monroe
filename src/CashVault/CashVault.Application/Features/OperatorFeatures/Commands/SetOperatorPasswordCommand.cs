using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.OperatorFeatures.Commands;

public class SetOperatorPasswordCommand : IRequest<Unit>
{
    public int OperatorId { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class SetOperatorPasswordCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SetOperatorPasswordCommand, Unit>
{
    public async Task<Unit> Handle(SetOperatorPasswordCommand command, CancellationToken cancellationToken)
    {
        var op = await unitOfWork.OperatorRepository.GetById<Operator>(command.OperatorId);

        if (op == null) throw new Exception();

        byte[] salt;
        var passwordHash = unitOfWork.OperatorRepository.HashPasword(command.Password, out salt);
        op.SetPassword(passwordHash, Convert.ToHexString(salt));

        unitOfWork.OperatorRepository.UpdateEntity(op);
        unitOfWork.SaveChanges();

        return Unit.Value;
    }
}
public class SetOperatorPasswordCommandValidator : AbstractValidator<SetOperatorPasswordCommand>
{
    public SetOperatorPasswordCommandValidator(ILocalizer t)
    {
        RuleFor(x => x.OperatorId).NotEmpty().WithMessage(t["OperatorId is required"]);
        RuleFor(x => x.Password).NotNull().WithMessage(t["Password is required."]);
        RuleFor(x => x.ConfirmPassword).NotNull().WithMessage(t["ConfirmPassword is required"]);
        RuleFor(x => x.Password).Equal(x => x.ConfirmPassword).WithMessage(t["Passwords do not match"]);
    }
}