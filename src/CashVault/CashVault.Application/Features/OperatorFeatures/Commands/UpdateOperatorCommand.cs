using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.OperatorFeatures.Commands;

public class UpdateOperatorCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Remarks { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Company { get; set; }
    public List<Permission> Permissions { get; set; } = [];
}

public class UpdateOperatorCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOperatorCommand, Unit>
{
    public async Task<Unit> Handle(UpdateOperatorCommand command, CancellationToken cancellationToken)
    {
        var op = await unitOfWork.OperatorRepository.GetOperatorWithPermissionsAsync(command.Id);

        if (op == null) throw new Exception();

        op.UpdatePersonalData(command.FirstName?.Trim(),
                              command.LastName?.Trim(),
                              command.Email?.Trim(),
                              command.PhoneNumber?.Trim(),
                              command?.Remarks?.Trim(),
                              command?.Company?.Trim());

        op.UpdatePermissions(command.Permissions);

        unitOfWork.OperatorRepository.UpdateEntity(op);
        unitOfWork.SaveChanges();

        return Unit.Value;
    }
}
public class UpdateOperatorCommandValidator : AbstractValidator<UpdateOperatorCommand>
{
    public UpdateOperatorCommandValidator(ILocalizer t)
    {
        RuleFor(r => r.FirstName).NotEmpty()
            .WithMessage(t["First name is required."])
            .MinimumLength(2)
            .WithMessage(t["First name must be at least 2 characters long."])
            .MaximumLength(100)
            .WithMessage(t["First name must not exceed 100 characters."]);

        RuleFor(r => r.LastName).NotEmpty()
            .WithMessage(t["LastName name is required."])
            .MinimumLength(2)
            .WithMessage(t["LastName name must be at least 2 characters long."])
            .MaximumLength(100)
            .WithMessage(t["LastName name must not exceed 100 characters."]);


        RuleFor(r => r.PhoneNumber).NotEmpty()
            .WithMessage(t["Phone number is required."])
            .MinimumLength(4)
            .WithMessage(t["Phone number must be at least 4 characters long."])
            .MaximumLength(20)
            .WithMessage(t["Phone number must not exceed 20 characters."]);

        RuleFor(r => r.Email).NotEmpty()
            .WithMessage(t["Email is required."])
            .EmailAddress()
            .WithMessage(t["Invalid email format."]);


        RuleFor(r => r.Permissions).NotNull()
            .NotEmpty()
            .WithMessage(t["Permissions are required."]);
    }
}