using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public record EnableDeviceCommand : IRequest<bool>
{
    public DeviceType DeviceType { get; init; }
    public EnableDeviceCommand(DeviceType deviceType)
    {
        DeviceType = deviceType;
    }
}

public class EnableDeviceCommandHandler : IRequestHandler<EnableDeviceCommand, bool>
{
    private readonly ITerminal terminal;
    private readonly INotificationService notificationService;

    public EnableDeviceCommandHandler(ITerminal terminal, INotificationService notificationService)
    {
        this.terminal = terminal;
        this.notificationService = notificationService;
    }

    public async Task<bool> Handle(EnableDeviceCommand request, CancellationToken cancellationToken)
    {
        var operationUuid = Guid.NewGuid();
        bool result = false;

        try
        {
            result = await terminal.EnableDeviceAsync(request.DeviceType);

            if (result)
            {
                await notificationService.OperationExecuted(operationUuid, $"Successfully enabled {request.DeviceType}", isSuccess: true);
            }
            else
            {
                await notificationService.OperationExecuted(operationUuid, $"Error while enabling {request.DeviceType}", isSuccess: false);
            }
        }
        catch (Exception ex)
        {
            await notificationService.OperationExecuted(operationUuid, ex.Message, isSuccess: false);
        }

        return result;
    }
}

public class EnableDeviceCommandValidator : AbstractValidator<EnableDeviceCommand>
{
    public EnableDeviceCommandValidator()
    {
        RuleFor(request => request.DeviceType).NotNull().WithMessage("Device type is required");
    }
}
