using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateTerminalMainConfiguration : IRequest<Unit>
{
    public List<Item> Items { get; set; } = [];
    public string? DeviceName { get; set; }
    public List<string> TerminalTypes { get; set; } = [];
}

public class Item
{
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? Interface { get; set; }
}

public class UpdateTerminalMainConfigurationHandler : IRequestHandler<UpdateTerminalMainConfiguration, Unit>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ITerminal _terminal;
    private readonly IStartupService startupService;
    private readonly IDeviceDriverFactory _deviceDriverFactory;

    public UpdateTerminalMainConfigurationHandler(IUnitOfWork unitOfWork,
                                                  ITerminal terminal,
                                                  IStartupService startupService,
                                                  IDeviceDriverFactory deviceDriverFactory)
    {
        this.unitOfWork = unitOfWork;
        this._terminal = terminal;
        this.startupService = startupService;
        _deviceDriverFactory = deviceDriverFactory;
    }

    public async Task<Unit> Handle(UpdateTerminalMainConfiguration command, CancellationToken cancellationToken)
    {
        var items = command.Items;

        CheckForDuplicatePorts(items);

        var config = new MainConfiguration
        {
            DeviceName = command.DeviceName?.Trim() ?? string.Empty
        };

        var availableDeviceModels = _terminal.GetAvailableDeviceModels();
        var availablePorts = _terminal.GetAvailablePorts();

        List<TerminalType> terminalTypes =
            command.TerminalTypes
                .Select(x => Enumeration.GetByCode<TerminalType>(x))
                .OfType<TerminalType>()
                .ToList();

        List<DeviceType> supportedDevices = _terminal.TerminalTypeDeviceConfiguration.GetSupportedDevicesByTerminalTypes(terminalTypes);

        config.TerminalTypes = terminalTypes;

        var cabinet = items?.Find(x => x.Name == DeviceType.Cabinet.Code);
        if (cabinet != null && !string.IsNullOrEmpty(cabinet.Interface) && !string.IsNullOrEmpty(cabinet.Value) && supportedDevices.Contains(DeviceType.Cabinet))
        {
            (DeviceModel d, Port p) = GetDeviceModelAndPort(DeviceType.Cabinet, cabinet.Value, cabinet.Interface);
            config.CabinetType = d;
            config.CabinetInterface = p;
        }

        var billAcceptor = items?.Find(x => x.Name == DeviceType.BillAcceptor.Code);
        if (billAcceptor != null && !string.IsNullOrEmpty(billAcceptor.Interface) && !string.IsNullOrEmpty(billAcceptor.Value) && supportedDevices.Contains(DeviceType.BillAcceptor))
        {
            (DeviceModel d, Port p) = GetDeviceModelAndPort(DeviceType.BillAcceptor, billAcceptor.Value, billAcceptor.Interface);
            config.BillAcceptorType = d;
            config.BillAcceptorInterface = p;
        }

        var billDispenser = items?.Find(x => x.Name == DeviceType.BillDispenser.Code);
        if (billDispenser != null && !string.IsNullOrEmpty(billDispenser.Interface) && !string.IsNullOrEmpty(billDispenser.Value) && supportedDevices.Contains(DeviceType.BillDispenser))
        {
            (DeviceModel d, Port p) = GetDeviceModelAndPort(DeviceType.BillDispenser, billDispenser.Value, billDispenser.Interface);
            config.BillDispenserType = d;
            config.BillDispenserInterface = p;
        }

        var titoPrinter = items?.Find(x => x.Name == DeviceType.TITOPrinter.Code);
        if (titoPrinter != null && !string.IsNullOrEmpty(titoPrinter.Interface) && !string.IsNullOrEmpty(titoPrinter.Value) && supportedDevices.Contains(DeviceType.TITOPrinter))
        {
            (DeviceModel d, Port p) = GetDeviceModelAndPort(DeviceType.TITOPrinter, titoPrinter.Value, titoPrinter.Interface);
            config.TITOPrinterType = d;
            config.TITOPrinterInterface = p;
        }

        var userCardReader = items?.Find(x => x.Name == DeviceType.UserCardReader.Code);
        if (userCardReader != null && !string.IsNullOrEmpty(userCardReader.Interface) && !string.IsNullOrEmpty(userCardReader.Value) && supportedDevices.Contains(DeviceType.UserCardReader))
        {
            (DeviceModel d, Port p) = GetDeviceModelAndPort(DeviceType.UserCardReader, userCardReader.Value, userCardReader.Interface);
            config.UserCardReaderType = d;
            config.UserCardReaderInterface = p;
        }

        var coinAcceptor = items?.Find(x => x.Name == DeviceType.CoinAcceptor.Code);
        if (coinAcceptor != null && !string.IsNullOrEmpty(coinAcceptor.Interface) && !string.IsNullOrEmpty(coinAcceptor.Value) && supportedDevices.Contains(DeviceType.CoinAcceptor))
        {
            (DeviceModel d, Port p) = GetDeviceModelAndPort(DeviceType.CoinAcceptor, coinAcceptor.Value, coinAcceptor.Interface);
            config.CoinAcceptorType = d;
            config.CoinAcceptorInterface = p;
        }

        var parcelLocker = items?.Find(x => x.Name == DeviceType.ParcelLocker.Code);
        if (parcelLocker != null && !string.IsNullOrEmpty(parcelLocker.Interface) && !string.IsNullOrEmpty(parcelLocker.Value) && supportedDevices.Contains(DeviceType.ParcelLocker))
        {
            (DeviceModel d, Port p) = GetDeviceModelAndPort(DeviceType.ParcelLocker, parcelLocker.Value, parcelLocker.Interface);
            config.ParcelLockerType = d;
            config.ParcelLockerInterface = p;
        }

        unitOfWork.TerminalRepository.UpdateMainConfigurationAsync(config);
        await unitOfWork.SaveChangesAsync();

        _deviceDriverFactory.SetMainConfiguration(config);
        _terminal.SetMainConfiguration(config);
        _terminal.StopAsync();
        _terminal.StartAsync();
        startupService.AddEventDispatching(_terminal);

        return Unit.Value;
    }

    private (DeviceModel?, Port?) GetDeviceModelAndPort(DeviceType deviceType, string driverName, string portName)
    {
        var deviceModel = _terminal.GetAvailableDeviceModels().Find(m => m.DeviceType == deviceType && m.FullyQualifiedDriverName.Equals(driverName));
        var port = _terminal.GetAvailablePorts().Find(p => p.Name.Equals(portName));

        if (deviceModel == null && driverName != "---")
        {
            throw new Exception($"Invalid device model for {deviceType}");
        }

        if (port == null && portName != "---")
        {
            throw new Exception($"Invalid port for {deviceType}");
        }

        return (deviceModel, port);
    }

    private static void CheckForDuplicatePorts(List<Item> items)
    {
        var ports = items.Where(x => x.Interface != null && x.Interface != "---").Select(x => x.Interface).ToList();
        var portsLookup = ports.ToLookup(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

        if (portsLookup.Any())
        {
            throw new ArgumentException("Duplicate ports");
        }
    }
}
public class UpdateTerminalMainConfigurationValidator : AbstractValidator<UpdateTerminalMainConfiguration>
{
    public UpdateTerminalMainConfigurationValidator(ITerminal terminal, ILocalizer t)
    {
        List<string> availableDeviceModels = terminal.GetAvailableDeviceModels().Select(m => m.FullyQualifiedDriverName).ToList();
        availableDeviceModels.Add("---");

        var availablePorts = terminal.GetAvailablePorts().Select(p => p.Name).ToList();
        availablePorts.Add("---");

        RuleFor(x => x.DeviceName).MaximumLength(100).WithMessage(t["Device name cannot exceed 100 characters."]);
    }
}
