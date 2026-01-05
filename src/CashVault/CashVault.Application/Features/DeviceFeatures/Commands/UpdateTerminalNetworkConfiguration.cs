using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateTerminalNetworkConfiguration : IRequest<Unit>
{
    public string Name { get; set; } = null!;
    public bool IsDhcpEnabled { get; set; }
    public bool IsDnsEnabled { get; set; }
    public string? IpAddress { get; set; }
    public string? NetworkMask { get; set; }
    public string? Gateway { get; set; }
    public string? PreferredDns { get; set; }
    public string? AlternateDns { get; set; }
}

public class UpdateTerminalNetworkConfigurationHandler : IRequestHandler<UpdateTerminalNetworkConfiguration, Unit>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ITerminal terminal;
    private readonly ILogger<UpdateTerminalNetworkConfigurationHandler> logger;

    public UpdateTerminalNetworkConfigurationHandler(IUnitOfWork unitOfWork, ITerminal terminal, ILogger<UpdateTerminalNetworkConfigurationHandler> logger)
    {
        this.unitOfWork = unitOfWork;
        this.terminal = terminal;
        this.logger = logger;
    }

    public async Task<Unit> Handle(UpdateTerminalNetworkConfiguration command, CancellationToken cancellationToken)
    {
        var networkConfig = await unitOfWork.TerminalRepository.GetCurrentNetworkConfigurationAsync();

        var adapterConfig = new NetworkAdapterConfiguration(command.Name,
                                                            command.IsDhcpEnabled,
                                                            command.IsDnsEnabled,
                                                            command.IpAddress,
                                                            command.NetworkMask,
                                                            command.Gateway,
                                                            command.PreferredDns,
                                                            command.AlternateDns);

        networkConfig.UpdateNetworkAdapterConfiguration(adapterConfig);


        unitOfWork.TerminalRepository.UpdateNetworkConfigurationAsync(networkConfig);
        await unitOfWork.SaveChangesAsync();

        string result = terminal.SetNetworkConfiguration(networkConfig);

        if (!string.IsNullOrEmpty(result))
        {
            logger.LogInformation(result);
        }

        return Unit.Value;
    }
}

public class UpdateTerminalNetworkConfigurationValidator : AbstractValidator<UpdateTerminalNetworkConfiguration>
{
    public UpdateTerminalNetworkConfigurationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Name is required.")
            .Must(x => NetworkConfiguration.GetSupportedNetworkInterfaces().Any(ni => ni.Name == x))
            .WithMessage("Unsupported Network adapter.");

        RuleFor(x => x.IsDhcpEnabled)
            .NotNull().WithMessage("DhcpEnabled is required.");

        RuleFor(x => x.IpAddress)
            .Must(NetworkConfiguration.IsValidIpAddress)
            .When(x => !x.IsDhcpEnabled)
            .WithMessage("Invalid IP address format.");

        RuleFor(x => x.NetworkMask)
            .NotEmpty().WithMessage("Network mask is required when DHCP is disabled.")
            .Must(x => !string.IsNullOrEmpty(x) && NetworkConfiguration.IsValidMask(x))
            .When(x => !x.IsDhcpEnabled)
            .WithMessage("Invalid network mask format.");

        RuleFor(x => x.Gateway)
            .NotEmpty().WithMessage("Gateway is required when DHCP is disabled.")
            .Must(NetworkConfiguration.IsValidIpAddress)
            .When(x => !x.IsDhcpEnabled)
            .WithMessage("Invalid gateway format.");

        RuleFor(x => x.PreferredDns)
            .NotEmpty().WithMessage("Preferred DNS is required.")
            .When(x => !x.IsDnsEnabled)
            .Must(NetworkConfiguration.IsValidIpAddress)
            .When(x => !x.IsDnsEnabled)
            .WithMessage("Must be valid address.");
    }
}
