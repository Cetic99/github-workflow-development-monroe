using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Common.Behaviors
{
    /// <summary>
    /// Notification publisher that prevents concurrent execution of events implementing ISynchronizationEvent.
    /// Ensures that only one synchronized event is processed at a time by leveraging ITerminal transaction control.
    /// </summary>
    public class EventSynchronizationBehavior : INotificationPublisher
    {
        private readonly ITerminal _terminal;
        private readonly ILogger<EventSynchronizationBehavior> _logger;

        public EventSynchronizationBehavior(ITerminal terminal, ILogger<EventSynchronizationBehavior> logger)
        {
            _terminal = terminal;
            _logger = logger;
        }

        public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
        {

            if (handlerExecutors == null || !handlerExecutors.Any())
            {
                return;
            }

            foreach (var handler in handlerExecutors)
            {
                if (notification is not ISynchronizationEvent)
                {
                    await handler
                   .HandlerCallback(notification, cancellationToken)
                   .ConfigureAwait(false);

                    continue;
                }

                _logger.LogDebug("Concurrent event execution started.");

                bool isStarted = await _terminal.StartTransaction();

                if (!isStarted)
                {
                    _logger.LogError("Concurrent event execution already started.");
                    continue;
                }

                try
                {
                    await handler
                    .HandlerCallback(notification, cancellationToken)
                    .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogDebug(e, "Concurrent event execution ended with exception.");
                }
                finally
                {
                    await _terminal.EndTransaction();
                    _logger.LogDebug("Concurrent command execution ended.");
                }
            }
        }
    }
}
