using System.Text.Json;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateBillDispenserConfigurationCommand : IRequest<Unit>
{
    public JsonDocument Config { get; set; }

    public UpdateBillDispenserConfigurationCommand(JsonDocument config)
    {
        Config = config;
    }
}

public class UpdateBillDispenserConfigurationCommandHandler : IRequestHandler<UpdateBillDispenserConfigurationCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITerminal _terminal;
    private readonly IDeviceDriverFactory _deviceDriverFactory;

    public UpdateBillDispenserConfigurationCommandHandler(IUnitOfWork unitOfWork, ITerminal terminal, IDeviceDriverFactory deviceDriverFactory)
    {
        _unitOfWork = unitOfWork;
        _terminal = terminal;
        _deviceDriverFactory = deviceDriverFactory;
    }

    public async Task<Unit> Handle(UpdateBillDispenserConfigurationCommand command, CancellationToken cancellationToken)
    {
        var dispenserBillCountStatus = await _unitOfWork.MoneyStatusRepository.GetDispenserBillCountStatusAsync();
        var config = (IBillDispenserConfiguration)_deviceDriverFactory.CreateConfiguration(DeviceType.BillDispenser, command.Config);
        config.Validate();

        //====> bill dispenser is not empty
        if (dispenserBillCountStatus != null && dispenserBillCountStatus.Cassettes.Any(x => x.CurrentBillCount > 0))
        {
            _unitOfWork.TerminalRepository.UpdateBillDispenserConfigurationAsync(config);
            await _unitOfWork.SaveChangesAsync();
            _terminal.SetBillDispenserConfiguration(config);

            return Unit.Value;
        }

        _unitOfWork.TerminalRepository.UpdateBillDispenserConfigurationAsync(config);

        var billStatus = await _unitOfWork.MoneyStatusRepository.GetDispenserBillCountStatusAsync();

        billStatus.UpdateBillCassetteFromDispenserConfig(config.BillCassettes);
        _unitOfWork.MoneyStatusRepository.UpdateDispenserBillCountStatus(billStatus);

        await _unitOfWork.SaveChangesAsync();

        _terminal.SetBillDispenserConfiguration(config);
        return Unit.Value;
    }
}