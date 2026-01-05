using CashVault.Application.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class ScanUserCardCommand
    : IRequest<Unit>
{ }

internal sealed class ScanUserCardCommandHandler(
    INotificationService notificationService)
    : IRequestHandler<ScanUserCardCommand, Unit>
{
    public async Task<Unit> Handle(ScanUserCardCommand command, CancellationToken cancellationToken)
    {
        await Task.Delay(3_000); // Simulate some delay for scanning

        // TODO: Implement actual card scanning logic here
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        await notificationService.CardScanCompleted();

        return Unit.Value;
    }
}
