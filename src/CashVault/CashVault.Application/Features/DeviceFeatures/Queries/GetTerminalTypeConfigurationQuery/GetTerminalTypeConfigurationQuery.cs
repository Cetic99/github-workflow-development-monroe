using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries;

public class GetTerminalTypeConfigurationQuery : IRequest<TerminalTypeConfigurationDto> { }

internal sealed class GetTerminalTypeConfigurationQueryHandler : IRequestHandler<GetTerminalTypeConfigurationQuery, TerminalTypeConfigurationDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITerminal _terminal;

    public GetTerminalTypeConfigurationQueryHandler(IUnitOfWork unitOfWork, ITerminal terminal)
    {
        _unitOfWork = unitOfWork;
        _terminal = terminal;
    }

    public async Task<TerminalTypeConfigurationDto> Handle(GetTerminalTypeConfigurationQuery query, CancellationToken cancellationToken)
    {
        TerminalTypeConfigurationDto result = new();

        MainConfiguration? mainConfig = await _unitOfWork.TerminalRepository.GetCurrentMainConfigurationAsync();

        result.TerminalTypeToDevicesMap = _terminal.TerminalTypeDeviceConfiguration.GetConfig().ToDictionary(
            kvp => kvp.Key.Code,
            kvp => kvp.Value.Select(x => x.Code).ToList());

        result.SupportedDevices = mainConfig?.TerminalTypes?
            .SelectMany(tt => _terminal.TerminalTypeDeviceConfiguration.GetSupportedDevicesByTerminalTypes([tt]))
            .Distinct()
            .Select(dt => dt.Code)
            .ToList() ?? [];

        result.SelectedTerminalTypes = mainConfig?.TerminalTypes?
            .Select(tt => tt.Code)
            .ToList() ?? [];

        return result;
    }
}
