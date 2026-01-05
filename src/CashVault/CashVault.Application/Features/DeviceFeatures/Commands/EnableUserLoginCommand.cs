using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class EnableUserLoginCommand : IRequest<Unit> { }

internal sealed class EnableUserLoginCommandHandler : IRequestHandler<EnableUserLoginCommand, Unit>
{
    private readonly ITerminal _terminal;
    private readonly ILogger<EnableUserLoginCommandHandler> _logger;
    public EnableUserLoginCommandHandler(ITerminal terminal, ILogger<EnableUserLoginCommandHandler> logger)
    {
        _terminal = terminal;
        _logger = logger;
    }
    public Task<Unit> Handle(EnableUserLoginCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling enable user login");

        _terminal.SetUserLoginEnabled(true);
        return Task.FromResult(Unit.Value);
    }
}
