using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class InitUserIdentificationCardReaderCommand
    : IRequest<Unit>
{ }

internal sealed class InitUserIdentificationCardReaderCommandHandler(
    ITerminal terminal,
    INotificationService notificationService)
    : IRequestHandler<InitUserIdentificationCardReaderCommand, Unit>
{
    public async Task<Unit> Handle(InitUserIdentificationCardReaderCommand command, CancellationToken cancellationToken)
    {
        await Task.Delay(3_000); // Simulate some delay for initialization

        if (terminal.UserCardReader is null || !terminal.UserCardReader.IsEnabled)
            await notificationService.CardReaderInitialzationFailed();
        else await notificationService.CardReaderInitialized();

        return Unit.Value;
    }
}
