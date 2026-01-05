using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands
{
    public record ResetDeviceCommand : IRequest<bool>
    {
        public DeviceType DeviceType { get; init; }

        public ResetDeviceCommand(DeviceType deviceType)
        {
            DeviceType = deviceType;
        }
    }

    internal sealed class ResetDeviceCommandHandler : IRequestHandler<ResetDeviceCommand, bool>
    {
        private readonly ITerminal terminal;
        private readonly INotificationService notificationService;

        public ResetDeviceCommandHandler(ITerminal terminal, INotificationService notificationService)
        {
            this.terminal = terminal;
            this.notificationService = notificationService;
        }

        public async Task<bool> Handle(ResetDeviceCommand request, CancellationToken cancellationToken)
        {
            var operationUuid = Guid.NewGuid();
            var result = false;

            try
            {
                result = await terminal.ResetDeviceAsync(request.DeviceType);
                if (result)
                {
                    await notificationService.OperationExecuted(operationUuid, $"Successfully reset {request.DeviceType}", isSuccess: true);
                }
                else
                {
                    await notificationService.OperationExecuted(operationUuid, $"Error while resetting {request.DeviceType}", isSuccess: false);
                }
            }
            catch (Exception ex)
            {
                await notificationService.OperationExecuted(operationUuid, ex.Message, isSuccess: false);
            }

            return result;
        }
    }
}
