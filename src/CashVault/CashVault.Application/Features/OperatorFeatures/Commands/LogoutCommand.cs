using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.OperatorFeatures.Commands;

public class LogoutCommand : IRequest<Unit> { }

public class LogoutCommandHandler
    (ILogger<LogoutCommandHandler> logger,
     ITerminal terminal)
        : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (terminal == null)
            {
                throw new InvalidOperationException("Terminal configuration is not set");
            }

            terminal.SetOperatingMode(TerminalOperatingMode.UnknownUser);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }

        return Unit.Value;
    }
}

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator() { }
}