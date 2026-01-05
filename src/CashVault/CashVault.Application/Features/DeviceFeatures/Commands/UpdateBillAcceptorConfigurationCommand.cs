using System.Text.Json;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateBillAcceptorConfigurationCommand : IRequest<Unit>
{
    public JsonDocument Config { get; set; }

    public UpdateBillAcceptorConfigurationCommand(JsonDocument config)
    {
        Config = config;
    }
}

public class UpdateBillAcceptorConfigurationCommandHandler
    : IRequestHandler<UpdateBillAcceptorConfigurationCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITerminal _terminal;
    private readonly IDeviceDriverFactory _deviceDriverFactory;

    public UpdateBillAcceptorConfigurationCommandHandler(IUnitOfWork unitOfWork, ITerminal terminal, IDeviceDriverFactory deviceDriverFactory)
    {
        _unitOfWork = unitOfWork;
        _terminal = terminal;
        _deviceDriverFactory = deviceDriverFactory;
    }

    public async Task<Unit> Handle(UpdateBillAcceptorConfigurationCommand command, CancellationToken cancellationToken)
    {
        var config = (IBillAcceptorConfiguration)_deviceDriverFactory.CreateConfiguration(DeviceType.BillAcceptor, command.Config);
        config.Validate();

        _unitOfWork.TerminalRepository.UpdateBillAcceptorConfigurationAsync(config);
        _terminal.SetBillAcceptorConfiguration(config);

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
