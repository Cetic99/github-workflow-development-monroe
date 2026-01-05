using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands
{
    public record ResetAllDevicesCommand : IRequest<bool> { }

    internal sealed class ResetAllDevicesCommandHandler : IRequestHandler<ResetAllDevicesCommand, bool>
    {
        private readonly ITerminal terminal;
        private readonly IStartupService startupService;
        private readonly INotificationService notificationService;

        public ResetAllDevicesCommandHandler(ITerminal terminal, IStartupService startupService, INotificationService notificationService)
        {
            this.terminal = terminal;
            this.startupService = startupService;
            this.notificationService = notificationService;
        }

        public async Task<bool> Handle(ResetAllDevicesCommand request, CancellationToken cancellationToken)
        {
            var operationUuid = Guid.NewGuid();
            bool result = false;

            try
            {
                await terminal.ResetAsync();
                startupService.AddEventDispatching(terminal);
                await notificationService.OperationExecuted(operationUuid, "Successfully reset all devices", isSuccess: true);
                result = true;
            }
            catch (Exception ex)
            {
                await notificationService.OperationExecuted(operationUuid, ex.Message, isSuccess: false);
            }

            return result;
        }
    }
}
