using System.Text.Json;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateCoinAcceptorConfigurationCommand : IRequest<Unit>
{
    public JsonDocument Config { get; set; }

    public UpdateCoinAcceptorConfigurationCommand(JsonDocument config)
    {
        Config = config;
    }
}

internal sealed class UpdateCoinAcceptorConfigurationCommandHandler : IRequestHandler<UpdateCoinAcceptorConfigurationCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITerminal _terminal;
    private readonly IDeviceDriverFactory _deviceDriverFactory;

    public UpdateCoinAcceptorConfigurationCommandHandler(IUnitOfWork unitOfWork, ITerminal terminal, IDeviceDriverFactory deviceDriverFactory)
    {
        _unitOfWork = unitOfWork;
        _terminal = terminal;
        _deviceDriverFactory = deviceDriverFactory;
    }

    public async Task<Unit> Handle(UpdateCoinAcceptorConfigurationCommand command, CancellationToken cancellationToken)
    {
        var config = (ICoinAcceptorConfiguration)_deviceDriverFactory.CreateConfiguration(DeviceType.CoinAcceptor, command.Config);
        config.Validate();

        _unitOfWork.TerminalRepository.UpdateCoinAcceptorConfigurationAsync(config);
        _terminal.SetCoinAcceptorConfiguration(config);

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}

