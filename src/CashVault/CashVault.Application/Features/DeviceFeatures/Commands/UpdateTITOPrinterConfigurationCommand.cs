using System.Text.Json;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common.Exceptions;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public record UpdateTITOPrinterConfigurationCommand : IRequest<Unit>
{
    public JsonDocument Config { get; set; }

    public UpdateTITOPrinterConfigurationCommand(JsonDocument config)
    {
        Config = config;
    }
}

public class UpdateTITOPrinterConfigurationCommandHandler : IRequestHandler<UpdateTITOPrinterConfigurationCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITerminal _terminal;
    private readonly IDeviceDriverFactory _deviceDriverFactory;

    public UpdateTITOPrinterConfigurationCommandHandler(IUnitOfWork unitOfWork, ITerminal terminal, IDeviceDriverFactory deviceDriverFactory)
    {
        _unitOfWork = unitOfWork;
        _terminal = terminal;
        _deviceDriverFactory = deviceDriverFactory;
    }

    public async Task<Unit> Handle(UpdateTITOPrinterConfigurationCommand command, CancellationToken cancellationToken)
    {
        var config = (ITITOPrinterConfiguration)_deviceDriverFactory.CreateConfiguration(DeviceType.TITOPrinter, command.Config);
        config.Validate();

        bool IsCMSEnabled = _terminal.OnlineIntegrationsConfiguration != null && _terminal.OnlineIntegrationsConfiguration.CasinoManagementSystem;

        if (config.IsCasinoManagementSystem && !IsCMSEnabled)
        {
            throw new BaseException("CMS not enabled");
        }

        _terminal.SetTITOPrinterConfiguration(config);
        _unitOfWork.TerminalRepository.UpdateTITOPrinterConfigurationAsync(config);

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
public class UpdateTITOPrinterConfigurationCommandValidator : AbstractValidator<UpdateTITOPrinterConfigurationCommand>
{
    public UpdateTITOPrinterConfigurationCommandValidator(ILocalizer t)
    {
        RuleFor(x => x.Config).NotEmpty().NotNull().WithMessage(t["Configuration is required"]);
    }
}
