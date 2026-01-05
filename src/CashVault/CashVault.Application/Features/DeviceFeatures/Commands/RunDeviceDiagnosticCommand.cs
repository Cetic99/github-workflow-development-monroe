using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class RunDeviceDiagnosticCommand : IRequest<bool>
{
    public DeviceType DeviceType { get; set; }
    public string CommandCode { get; set; }
}

public class RunDeviceDiagnosticCommandValidator : AbstractValidator<RunDeviceDiagnosticCommand>
{
    public RunDeviceDiagnosticCommandValidator()
    {
        RuleFor(x => x.DeviceType)
            .NotNull()
            .WithMessage("Device type is required.");
        RuleFor(x => x.CommandCode)
            .NotEmpty()
            .WithMessage("Command code is required.");
    }
}

internal sealed class RunDeviceDiagnosticCommandHandler : IRequestHandler<RunDeviceDiagnosticCommand, bool>
{
    private readonly ITerminal _terminal;
    private readonly INotificationService _notificationService;

    public RunDeviceDiagnosticCommandHandler(ITerminal terminal, INotificationService notificationService)
    {
        _terminal = terminal;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(RunDeviceDiagnosticCommand command, CancellationToken cancellationToken)
    {
        var operationUuid = Guid.NewGuid();
        var result = new Domain.Common.OperationResult();

        var device = _terminal.GetDeviceByType(command.DeviceType)
            ?? throw new InvalidOperationException($"Device of type {command.DeviceType} not found.");

        if (_terminal.IsTransactionInProgress)
        {
            await _notificationService.OperationExecuted(operationUuid, "Cannot run diagnostic command while a transaction is in progress.", isSuccess: false);
        }

        try
        {
            result = await device.RunDiagnosticsCommand(new DeviceDiagnosticsCommand(command.CommandCode));
            if (result.IsSuccess)
            {
                await _notificationService.OperationExecuted(operationUuid, $"Successfully executed {command.CommandCode}", isSuccess: true);
            }
            else
            {
                await _notificationService.OperationExecuted(operationUuid, result.ErrorMessage ?? $"Error while executing {command.CommandCode}", isSuccess: false);
            }
        }
        catch (Exception ex)
        {
            await _notificationService.OperationExecuted(operationUuid, ex.Message, isSuccess: false);
        }

        return result.IsSuccess;
    }
}
