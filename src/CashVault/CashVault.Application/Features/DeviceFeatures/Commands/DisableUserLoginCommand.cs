using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class DisableUserLoginCommand : IRequest<Unit> { }

internal sealed class DisableUserLoginCommandHandler : IRequestHandler<DisableUserLoginCommand, Unit>
{
    private readonly ITerminal _terminal;
    private readonly ILogger<DisableUserLoginCommandHandler> _logger;

    public DisableUserLoginCommandHandler(ITerminal terminal, ILogger<DisableUserLoginCommandHandler> logger)
    {
        _terminal = terminal;
        _logger = logger;
    }

    public Task<Unit> Handle(DisableUserLoginCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling disable user login");

        _terminal.SetUserLoginEnabled(false);
        return Task.FromResult(Unit.Value);
    }
}