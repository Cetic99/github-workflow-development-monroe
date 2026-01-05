using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateTerminalOnlineIntegrationsConfiguration : IRequest<Unit>
{
    public bool CasinoManagementSystem { get; set; }
    public string? Url { get; set; }
    public string? DeviceId { get; set; }
    public string? SecretKey { get; set; }
    public int TimeoutInSeconds { get; set; }
}

public class UpdateTerminalOnlineIntegrationsConfigurationHandler(IUnitOfWork unitOfWork, ITerminal terminal)
    : IRequestHandler<UpdateTerminalOnlineIntegrationsConfiguration, Unit>
{
    public async Task<Unit> Handle(UpdateTerminalOnlineIntegrationsConfiguration command, CancellationToken cancellationToken)
    {
        var config = new OnlineIntegrationsConfiguration()
        {
            CasinoManagementSystem = command.CasinoManagementSystem,
            Url = command.Url,
            DeviceId = command.DeviceId,
            SecretKey = command.SecretKey,
            TimeoutInSeconds = command.TimeoutInSeconds
        };

        unitOfWork.TerminalRepository.UpdateOnlineIntegrationsConfigurationAsync(config);
        await unitOfWork.SaveChangesAsync();

        terminal.SetOnlineIntegrationsConfiguration(config);

        return Unit.Value;
    }
}
public class UpdateTerminalOnlineIntegrationsConfigurationValidator : AbstractValidator<UpdateTerminalOnlineIntegrationsConfiguration>
{
    public UpdateTerminalOnlineIntegrationsConfigurationValidator(ILocalizer t)
    {
        When(x => x.CasinoManagementSystem, () =>
        {
            RuleFor(x => x.Url).NotEmpty().WithMessage(t["Url is required."]);
            RuleFor(x => x.DeviceId).NotEmpty().WithMessage(t["DeviceId is required."]);
            RuleFor(x => x.TimeoutInSeconds)
                .GreaterThan(0).WithMessage(t["Must be greater than 0"])
                .LessThanOrEqualTo(100).WithMessage(t["Must be less or equal than {0}", 100]);
        });
    }
}
