using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.OperatorAggregate;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.OperatorFeatures.Commands;

public class AuthenticateOperatorWithCredentialsCommand : IRequest<Unit>
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class AuthenticateOperatorWithCredentialsCommandHandler
    (IUnitOfWork unitOfWork,
     INotificationService notificationService,
     IAuthenticationService authenticationService,
     ILogger<AuthenticateOperatorWithCredentialsCommandHandler> logger,
     ITerminal terminal)
        : IRequestHandler<AuthenticateOperatorWithCredentialsCommand, Unit>
{
    public async Task<Unit> Handle(AuthenticateOperatorWithCredentialsCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(command.Username))
                throw new ArgumentNullException("Username", "Username null or empty");

            if (string.IsNullOrEmpty(command.Password))
                throw new ArgumentNullException("Password", "Password null or empty");

            Operator @operator = await
                unitOfWork.OperatorRepository.GetOperatorByUsernameAsync(command.Username);

            if (@operator == null)
                throw new ArgumentNullException("Operator", $"Operator ({command.Username}) null or empty");

            var passwordVerified =
                authenticationService.VerifyPassword(@operator, command.Password);

            @operator.AuthenticateWithCredentials(passwordVerified);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            if (ex is not InvalidOperationException) await notificationService.AuthenicationFailed();
        }
        finally
        {
            await unitOfWork.SaveChangesAsync();
        }

        return Unit.Value;
    }
}

public class AuthenticateOperatorWithCredentialsCommandValidator : AbstractValidator<AuthenticateOperatorWithCredentialsCommand>
{
    public AuthenticateOperatorWithCredentialsCommandValidator(ILocalizer t)
    {
        RuleFor(r => r.Username).NotEmpty().WithMessage(t["Username is required."]);
        RuleFor(r => r.Password).NotEmpty().WithMessage(t["Password is required."]);
    }
}