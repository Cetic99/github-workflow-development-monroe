using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries;

public class GetDeviceDiagnosticCommandsQuery : IRequest<List<DeviceDiagnosticCommand>>
{
    public DeviceType DeviceType { get; init; } = null!;

    public GetDeviceDiagnosticCommandsQuery(DeviceType deviceType)
    {
        DeviceType = deviceType;
    }
}

public class GetDeviceDiagnosticCommandsQueryValidator : AbstractValidator<GetDeviceDiagnosticCommandsQuery>
{
    public GetDeviceDiagnosticCommandsQueryValidator()
    {
        RuleFor(x => x.DeviceType)
            .NotNull()
            .WithMessage("Device type is required.");
    }
}

internal sealed class GetDeviceDiagnosticCommandsQueryHandler : IRequestHandler<GetDeviceDiagnosticCommandsQuery, List<DeviceDiagnosticCommand>>
{
    private readonly ITerminal _terminal;
    private readonly ILocalizer t;

    public GetDeviceDiagnosticCommandsQueryHandler(ITerminal terminal, ILocalizer localizer)
    {
        _terminal = terminal;
        t = localizer;
    }

    public Task<List<DeviceDiagnosticCommand>> Handle(GetDeviceDiagnosticCommandsQuery request, CancellationToken cancellationToken)
    {
        var device = _terminal.GetDeviceByType(request.DeviceType)
            ?? throw new InvalidOperationException($"Device of type {request.DeviceType} not found.");

        var commands = new List<DeviceDiagnosticCommand>();

        foreach (var command in device.SupportedDiagnosticCommands)
        {
            commands.Add(new DeviceDiagnosticCommand()
            {
                Name = t[command.Code],
                Code = command.Code,
            });
        }

        return Task.FromResult(commands);
    }
}
