using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.OperatorFeatures.Commands;

public class AddOperatorCommand : IRequest<Unit>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string? Remarks { get; set; }
    public List<Permission> Permissions { get; set; } = [];
}

public class AddOperatorCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AddOperatorCommand, Unit>
{
    public async Task<Unit> Handle(AddOperatorCommand command, CancellationToken cancellationToken)
    {
        string username = command!.Username!.Trim()!;

        var userExistsByUsername = await unitOfWork.OperatorRepository.GetOperatorByUsernameAsync(username);

        if (userExistsByUsername != null)
        {
            throw new InvalidOperationException($"An operator with username '{username}' already exists.");
        }

        byte[] salt;
        var passwordHash =
            unitOfWork.OperatorRepository
                .HashPasword(unitOfWork.OperatorRepository.GetDefaultPassword(), out salt);

        var opr =
            new Operator
                (Guid.NewGuid(),
                 username,
                 command.FirstName.Trim(),
                 command.LastName?.Trim(),
                 command.Email?.Trim(),
                 command.PhoneNumber?.Trim(),
                 passwordHash,
                 Convert.ToHexString(salt),
                 command.Permissions,
                 command.Remarks?.Trim());

        unitOfWork.OperatorRepository.AddEntity(opr);
        unitOfWork.SaveChanges();

        return Unit.Value;
    }
}
public class AddOperatorCommandValidator : AbstractValidator<AddOperatorCommand>
{
    public AddOperatorCommandValidator(ILocalizer t)
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


        RuleFor(r => r.Username).NotEmpty()
            .WithMessage(t["Username is required."])
            .MinimumLength(4)
            .WithMessage(t["Username must be at least 4 characters long."])
            .MaximumLength(50)
            .WithMessage(t["Username must not exceed 50 characters."]);


        RuleFor(r => r.Email).NotEmpty()
            .WithMessage(t["Email is required."])
            .EmailAddress()
            .WithMessage(t["Invalid email format."]);


        RuleFor(r => r.Permissions).NotNull()
            .NotEmpty()
            .WithMessage(t["Permissions are required."]);
    }
}