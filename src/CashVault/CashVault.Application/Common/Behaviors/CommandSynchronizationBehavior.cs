using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline behavior that prevents concurrent execution of commands implementing ISynchronizationCommand.
    /// Ensures that only one command can be processed at a time by leveraging ITerminal transaction control.
    /// </summary>
    public class CommandSynchronizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ISynchronizationCommand
    {
        private readonly ITerminal _terminal;
        private readonly ILogger<CommandSynchronizationBehavior<TRequest, TResponse>> _logger;

        public CommandSynchronizationBehavior(ITerminal terminal, ILogger<CommandSynchronizationBehavior<TRequest, TResponse>> logger)
        {
            _terminal = terminal;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            _logger.LogDebug("Concurrent command execution started...");

            bool isStarted = await _terminal.StartTransaction();

            if (!isStarted)
            {
                _logger.LogDebug("Concurrent command execution already started...");
                throw new InvalidOperationException("Transaction is already started");
            }

            TResponse? response = default;

            try
            {
                response = await next();
            }
            catch (Exception)
            {
                await _terminal.EndTransaction();
                _logger.LogDebug("Concurrent command execution ended with exception...");
                throw;
            }
            finally
            {
                await _terminal.EndTransaction();
                _logger.LogDebug("Concurrent command execution ended...");
            }

            return response;
        }
    }
}
