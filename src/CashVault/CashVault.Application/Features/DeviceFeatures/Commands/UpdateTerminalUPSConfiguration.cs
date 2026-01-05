using FluentValidation;
using MediatR;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateTerminalUPSConfiguration : IRequest<Unit>
{
    public string? ConfiguredUpsType { get; set; }
    public int? ShutdownDelay { get; set; }
    public int? StartupDelay { get; set; }
    public int? UptimeAfterPowerShortage { get; set; }
}
public class UpdateTerminalUPSConfigurationHandler(IUnitOfWork unitOfWork, ITerminal terminal) 
    : IRequestHandler<UpdateTerminalUPSConfiguration, Unit>
{
    public async Task<Unit> Handle(UpdateTerminalUPSConfiguration command, CancellationToken cancellationToken)
    {
        var config = new UpsConfiguration()
        {
            ConfiguredUpsType = command.ConfiguredUpsType,
            ShutdownDelay = command.ShutdownDelay,
            StartupDelay = command.StartupDelay,
            UptimeAfterPowerShortage = command.UptimeAfterPowerShortage
        };

        unitOfWork.TerminalRepository.UpdateUpsConfigurationAsync(config);
        await unitOfWork.SaveChangesAsync();

        terminal.SetUpsConfiguration(config);

        return Unit.Value;
    }
}
public class UpdateTerminalUPSConfigurationValidator : AbstractValidator<UpdateTerminalUPSConfiguration>
{
    public UpdateTerminalUPSConfigurationValidator()
    {
        RuleFor(x => x.ConfiguredUpsType).NotEmpty();
        RuleFor(x => x.ShutdownDelay).NotEmpty();
        RuleFor(x => x.StartupDelay).NotEmpty();
        RuleFor(x => x.UptimeAfterPowerShortage).NotEmpty();
    }
}
