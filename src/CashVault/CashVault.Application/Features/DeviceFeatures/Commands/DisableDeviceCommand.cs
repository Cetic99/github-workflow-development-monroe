using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public record DisableDeviceCommand : IRequest<bool>
{
    public DeviceType DeviceType { get; init; }
    public DisableDeviceCommand(DeviceType deviceType)
    {
        DeviceType = deviceType;
    }
}

public class DisableDeviceCommandHandler : IRequestHandler<DisableDeviceCommand, bool>
{
    private readonly INotificationService notificationService;
    private readonly ITerminal terminal;

    public DisableDeviceCommandHandler(INotificationService notificationService, ITerminal terminal)
    {
        this.notificationService = notificationService;
        this.terminal = terminal;
    }

    public async Task<bool> Handle(DisableDeviceCommand request, CancellationToken cancellationToken)
    {
        var operationUuid = Guid.NewGuid();
        bool result = false;

        try
        {
            result = await terminal.DisableDeviceAsync(request.DeviceType);

            if (result)
            {
                await notificationService.OperationExecuted(operationUuid, $"Successfully disabled {request.DeviceType}", isSuccess: true);
            }
            else
            {
                await notificationService.OperationExecuted(operationUuid, $"Error while disabling {request.DeviceType}", isSuccess: false);
            }
        }
        catch (Exception ex)
        {
            await notificationService.OperationExecuted(operationUuid, ex.Message, isSuccess: false);
        }

        return result;
    }
}

public class DisableDeviceCommandValidator : AbstractValidator<DisableDeviceCommand>
{
    public DisableDeviceCommandValidator(ILocalizer t)
    {
        RuleFor(request => request.DeviceType).NotNull().WithMessage(t["Device type is required."]);
    }
}
