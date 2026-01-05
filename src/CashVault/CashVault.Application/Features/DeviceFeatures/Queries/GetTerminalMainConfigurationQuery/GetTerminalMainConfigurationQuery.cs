using CashVault.Application.Common.Models;
using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries;

public record GetTerminalMainConfigurationQuery : IRequest<TerminalMainConfigurationDto> { }

internal sealed class GetTerminalMainConfigurationQueryHandler : IRequestHandler<GetTerminalMainConfigurationQuery, TerminalMainConfigurationDto>
{
    private readonly ITerminalRepository _db;
    private readonly ITerminal _terminal;
    private readonly ILocalizer _t;

    public GetTerminalMainConfigurationQueryHandler(ITerminalRepository db, ITerminal terminal, ILocalizer t)
    {
        _db = db;
        _terminal = terminal;
        _t = t;
    }

    public async Task<TerminalMainConfigurationDto> Handle(GetTerminalMainConfigurationQuery request, CancellationToken cancellationToken)
    {
        var dto = new TerminalMainConfigurationDto(_t);
        dto.AvailablePorts.Add(new SelectListItem() { Name = _t["None"], Value = "---" });
        dto.AvailablePorts.AddRange(_terminal.GetAvailablePorts().Select(p => new SelectListItem() { Name = p.Name, Value = p.Name }).ToList());

        var availableDeviceModels = _terminal.GetAvailableDeviceModels().ToList();
        dto.AvailableDeviceModels = availableDeviceModels
            .GroupBy(x => x.DeviceType)
            .ToDictionary(
                g => g.Key.Code,
                g =>
                {
                    List<SelectListItem> list = [];
                    list.Add(new SelectListItem { Name = _t["None"], Value = "---" });
                    list.AddRange(g.Select(d => new SelectListItem
                    {
                        Value = d.FullyQualifiedDriverName.ToString(),
                        Name = d.Name
                    }).ToList());

                    return list;
                }
            );

        var config = await _db.GetCurrentMainConfigurationAsync();
        dto.DeviceName = config?.DeviceName ?? string.Empty;

        var terminalTypes = config?.TerminalTypes ?? [];

        dto.TerminalTypes = terminalTypes.Select(x => new SelectListItem()
        {
            Name = x.Code,
            Value = x.Code
        }).ToList() ?? [];

        var devices = _terminal.TerminalTypeDeviceConfiguration.GetSupportedDevicesByTerminalTypes(terminalTypes);

        foreach (var device in devices)
        {
            if (device.Code == DeviceType.BillAcceptor.Code && config.IsBillAcceptorMainConfigUpdated())
            {
                var billAcceptorModels = availableDeviceModels.Where(d => d.DeviceType == DeviceType.BillAcceptor).ToList();
                dto.Items.Add(new MainConfigurationItem()
                {
                    Label = _t["Bill acceptor type"],
                    Name = DeviceType.BillAcceptor.Code,
                    Value = config?.BillAcceptorType?.FullyQualifiedDriverName ?? "---",
                    Options = billAcceptorModels.Select(d => new SelectListItem() { Name = d.Name, Value = d.FullyQualifiedDriverName }).ToList(),
                    Interface = new MainConfigurationInterface()
                    {
                        Value = GetDevicePortName(config?.BillAcceptorInterface?.Name),
                        Options = _terminal.GetAvailablePorts().Select(p => new SelectListItem() { Name = p.Name, Value = p.Name }).ToList()
                    }
                });
            }
            else if (device.Code == DeviceType.UserCardReader.Code && config.IsUserCardReaderMainConfigUpdated())
            {
                var userCardReaderModels = availableDeviceModels.Where(d => d.DeviceType == DeviceType.UserCardReader).ToList();
                dto.Items.Add(new MainConfigurationItem()
                {
                    Label = _t["Card reader type"],
                    Name = DeviceType.UserCardReader.Code,
                    Value = config?.UserCardReaderType?.FullyQualifiedDriverName ?? "---",
                    Options = userCardReaderModels.Select(d => new SelectListItem() { Name = d.Name, Value = d.FullyQualifiedDriverName }).ToList(),
                    Interface = new MainConfigurationInterface()
                    {
                        Value = GetDevicePortName(config?.UserCardReaderInterface?.Name),
                        Options = _terminal.GetAvailablePorts().Select(p => new SelectListItem() { Name = p.Name, Value = p.Name }).ToList()
                    }
                });
            }
            else if (device.Code == DeviceType.TITOPrinter.Code && config.IsTITOPrinterMainConfigUpdated())
            {
                var printerModels = availableDeviceModels.Where(d => d.DeviceType == DeviceType.TITOPrinter).ToList();
                dto.Items.Add(new MainConfigurationItem()
                {
                    Label = _t["TITO Printer type"],
                    Name = DeviceType.TITOPrinter.Code,
                    Value = config?.TITOPrinterType?.FullyQualifiedDriverName ?? "---",
                    Options = printerModels.Select(d => new SelectListItem() { Name = d.Name, Value = d.FullyQualifiedDriverName }).ToList(),
                    Interface = new MainConfigurationInterface()
                    {
                        Value = GetDevicePortName(config?.TITOPrinterInterface?.Name),
                        Options = _terminal.GetAvailablePorts().Select(p => new SelectListItem() { Name = p.Name, Value = p.Name }).ToList()
                    }
                });
            }
            else if (device.Code == DeviceType.Cabinet.Code && config.IsCabinetMainConfigUpdated())
            {
                var cabinetModels = availableDeviceModels.Where(d => d.DeviceType == DeviceType.Cabinet).ToList();
                dto.Items.Add(new MainConfigurationItem()
                {
                    Label = _t["Cabinet type"],
                    Name = DeviceType.Cabinet.Code,
                    Value = config?.CabinetType?.FullyQualifiedDriverName ?? "---",
                    Options = cabinetModels.Select(d => new SelectListItem() { Name = d.Name, Value = d.FullyQualifiedDriverName }).ToList(),
                    Interface = new MainConfigurationInterface()
                    {
                        Value = GetDevicePortName(config?.CabinetInterface?.Name),
                        Options = _terminal.GetAvailablePorts().Select(p => new SelectListItem() { Name = p.Name, Value = p.Name }).ToList()
                    }
                });
            }
            else if (device.Code == DeviceType.BillDispenser.Code && config.IsBillDispenserMainConfigUpdated())
            {
                var billDispenserModels = availableDeviceModels.Where(d => d.DeviceType == DeviceType.BillDispenser).ToList();
                dto.Items.Add(new MainConfigurationItem()
                {
                    Label = _t["Bill dispenser type"],
                    Name = DeviceType.BillDispenser.Code,
                    Value = config?.BillDispenserType?.FullyQualifiedDriverName ?? "---",
                    Options = billDispenserModels.Select(d => new SelectListItem() { Name = d.Name, Value = d.FullyQualifiedDriverName }).ToList(),
                    Interface = new MainConfigurationInterface()
                    {
                        Value = GetDevicePortName(config?.BillDispenserInterface?.Name),
                        Options = _terminal.GetAvailablePorts().Select(p => new SelectListItem() { Name = p.Name, Value = p.Name }).ToList()
                    }
                });
            }
            else if (device.Code == DeviceType.CoinAcceptor.Code && config.IsCoinAcceptorMainConfigUpdated())
            {
                var coinAcceptorModels = availableDeviceModels.Where(d => d.DeviceType == DeviceType.CoinAcceptor).ToList();
                dto.Items.Add(new MainConfigurationItem()
                {
                    Label = _t["Coin dispenser type"],
                    Name = DeviceType.CoinAcceptor.Code,
                    Value = config?.CoinAcceptorType?.FullyQualifiedDriverName ?? "---",
                    Options = coinAcceptorModels.Select(d => new SelectListItem() { Name = d.Name, Value = d.FullyQualifiedDriverName }).ToList(),
                    Interface = new MainConfigurationInterface()
                    {
                        Value = GetDevicePortName(config?.CoinAcceptorInterface?.Name),
                        Options = _terminal.GetAvailablePorts().Select(p => new SelectListItem() { Name = p.Name, Value = p.Name }).ToList()
                    }
                });
            }
            else if (device.Code == DeviceType.ParcelLocker.Code && config.IsParcelLockerMainConfigUpdated())
            {
                var parcelLockerModels = availableDeviceModels.Where(d => d.DeviceType == DeviceType.ParcelLocker).ToList();
                dto.Items.Add(new MainConfigurationItem()
                {
                    Label = _t["Parcel locker type"],
                    Name = DeviceType.ParcelLocker.Code,
                    Value = config?.ParcelLockerType?.FullyQualifiedDriverName ?? "---",
                    Options = parcelLockerModels.Select(d => new SelectListItem() { Name = d.Name, Value = d.FullyQualifiedDriverName }).ToList(),
                    Interface = new MainConfigurationInterface()
                    {
                        Value = GetDevicePortName(config?.ParcelLockerInterface?.Name),
                        Options = _terminal.GetAvailablePorts().Select(p => new SelectListItem() { Name = p.Name, Value = p.Name }).ToList()
                    }
                });
            }
        }

        return await Task.FromResult(dto);
    }

    /// <summary>
    /// Retrieves the port name for a device, ensuring that the port name is valid and available.
    /// If the provided port name is invalid or unavailable, returns a default value ("---").
    /// </summary>
    private string GetDevicePortName(string? portName)
    {
        if (string.IsNullOrWhiteSpace(portName))
        {
            return "---";
        }

        if (_terminal.IsPortAvailable(portName))
        {
            return portName;
        }

        return "---";
    }
}
