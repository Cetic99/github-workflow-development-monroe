using CashVault.Application.Features.DeviceFeatures.Queries;
using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Aggregates.MessageAggregate;
using CashVault.Domain.Common;
using FluentValidation;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Commands;

public class UpdateTerminalRegionalConfiguration : IRequest<Unit>
{
    public TerminalRegionalConfigurationDto Data { get; set; } = null!;
}

public class UpdateTerminalRegionalConfigurationHandler
    : IRequestHandler<UpdateTerminalRegionalConfiguration, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITerminal _terminal;

    public UpdateTerminalRegionalConfigurationHandler(IUnitOfWork unitOfWork, ITerminal terminal)
    {
        _unitOfWork = unitOfWork;
        _terminal = terminal;
    }

    public async Task<Unit> Handle(UpdateTerminalRegionalConfiguration command, CancellationToken cancellationToken)
    {
        var config = new RegionalConfiguration()
        {
            PasswordStrongChecking = command.Data.PasswordStrongChecking,
            DaysLogFilesValid = command.Data.DaysLogFilesValid,
            MediumRedemption = command.Data.MediumRedemption,
            ValueFormat = command.Data.ValueFormat,
            CustomerNumber = command.Data.CustomerNumber,
            PropertyNumber = command.Data.PropertyNumber,
            AutomaticBackup = command.Data.AutomaticBackup,
            AutomaticLogout = command.Data.AutomaticLogout,
            BackupStartHour = command.Data.BackupStartHour,
            CasinoDayStarts = command.Data.CasinoDayStarts,
            Time = command.Data.Time,
            Distribution = command.Data.Distribution,
            SmallerNotes = command.Data.SmallerNotes,
            LocalTimeZone = command.Data.LocalTimeZone,
            PasswordLength = command.Data.PasswordLength,
            PasswordValid = command.Data.PasswordValid,
            LocationId = command.Data.LocationId,
            LogEvents = command.Data.LogEvents,
            Caption = command.Data.Caption,
            MachineName = command.Data.MachineName,
            LocationAddress = command.Data.LocationAddress,
            LocationName = command.Data.LocationName,
            DecimalSeparator = Enumeration.GetByCode<NumberSeparator>(command.Data.DecimalSeparator),
            ThousandSeparator = Enumeration.GetByCode<NumberSeparator>(command.Data.ThousandSeparator),
            DefaultLanguage = Enumeration.GetByCode<Language>(command.Data.DefaultLanguage)?.Code,
            DateFormat = Enumeration.GetByCode<DateFormat>(command.Data.DateFormat) ?? DateFormat.DefaultDateFormat,
            AmountPrecision = command.Data.AmountPrecision
        };

        //TODO: Add more properties for RegionalConfiguration

        _unitOfWork.TerminalRepository.UpdateRegionalConfigurationAsync(config);
        await _unitOfWork.SaveChangesAsync();

        _terminal.SetRegionalConfiguration(config);
        _terminal.UpdateLocalTimeZone(config.LocalTimeZone);
        _terminal.UpdateAmountPrecision(config.AmountPrecision);

        return Unit.Value;
    }
}
public class UpdateTerminalRegionalConfigurationValidator : AbstractValidator<UpdateTerminalRegionalConfiguration>
{
    public UpdateTerminalRegionalConfigurationValidator(ILocalizer t)
    {
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.LocationName)
            .NotEmpty().WithMessage(t["Location name is required"])
            .MinimumLength(5).WithMessage(t["Location name must be at least 3 characters long"]);

        RuleFor(x => x.Data.Caption)
            .NotEmpty().WithMessage(t["Caption is required"])
            .MinimumLength(3).WithMessage(t["Caption must be at least {0} characters long", 3]);

        RuleFor(x => x.Data.LocationAddress)
            .NotEmpty().WithMessage(t["Location address is required"])
            .MinimumLength(5).WithMessage(t["Location address must be at least {0} characters long", 5]);

        RuleFor(x => x.Data.MachineName)
            .NotEmpty().WithMessage(t["Machine number is required"])
            .MinimumLength(2).WithMessage(t["Machine number must be at least {0} characters long", 2]);

        //RuleFor(x => x.Data.CasinoDayStarts)
        //    .Matches(@"^\d{2}:\d{2}$").WithMessage(t["Day start time must be in the format hh:mm"])
        //    .When(x => !string.IsNullOrEmpty(x.Data.CasinoDayStarts) && x.Data.CasinoDayStarts != ":")
        //    .Must(IsValidTimeFormat).WithMessage(t["Day start time must be a valid time in the format hh:mm"])
        //    .When(x => !string.IsNullOrEmpty(x.Data.CasinoDayStarts) && x.Data.CasinoDayStarts != ":");
    }

    private bool IsValidTimeFormat(string time)
    {
        if (string.IsNullOrEmpty(time)) return false;
        var parts = time.Split(':');
        if (parts.Length != 2) return false;

        if (int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
        {
            return hours >= 0 && hours < 24 && minutes >= 0 && minutes < 60;
        }

        return false;
    }
}
