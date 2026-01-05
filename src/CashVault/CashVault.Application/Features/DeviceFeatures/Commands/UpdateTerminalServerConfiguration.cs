using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public record UpdateTerminalServerConfiguration : IRequest<Unit>
{
    public string Url { get; init; } = null!;
    public string DeviceId { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public int? MinimalLogLevel { get; set; }
}

public class UpdateTerminalServerConfigurationHandler(IUnitOfWork unitOfWork, ITerminal terminal)
    : IRequestHandler<UpdateTerminalServerConfiguration, Unit>
{
    public async Task<Unit> Handle(UpdateTerminalServerConfiguration command, CancellationToken cancellationToken)
    {
        var config = new ServerConfiguration()
        {
            ServerUrl = command.Url,
            DeviceId = command.DeviceId,
            IsEnabled = command.IsEnabled,
            MinimalLogLevel = command.MinimalLogLevel ?? (int)LogLevel.Information
        };

        unitOfWork.TerminalRepository.UpdateServerConfigurationAsync(config);
        await unitOfWork.SaveChangesAsync();

        terminal.SetServerConfiguration(config);

        return Unit.Value;
    }
}
public class UpdateTerminalServerConfigurationValidator : AbstractValidator<UpdateTerminalServerConfiguration>
{
    public UpdateTerminalServerConfigurationValidator(ILocalizer t)
    {
        RuleFor(x => x.Url).NotEmpty().WithMessage(t["Url is required."]);
        RuleFor(x => x.Url).Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute)).WithMessage(t["Url is not in correct format"]);
        RuleFor(x => x.DeviceId).NotEmpty().WithMessage(t["DeviceId is required."]);
        RuleFor(x => x.MinimalLogLevel)
            .Must(x => x == null || Enum.IsDefined(typeof(LogLevel), x))
            .When(x => x.MinimalLogLevel.HasValue)
            .WithMessage(t["Minimal log level is not valid."]);
    }
}
