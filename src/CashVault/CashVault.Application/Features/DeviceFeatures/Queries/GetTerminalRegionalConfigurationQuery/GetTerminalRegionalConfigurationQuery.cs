using CashVault.Application.Common.Models;
using CashVault.Application.Interfaces;
using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Common;
using MediatR;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public record GetTerminalRegionalConfigurationQuery : IRequest<TerminalRegionalConfigurationDto> { }

    internal sealed class GetTerminalRegionalConfigurationQueryHandler : IRequestHandler<GetTerminalRegionalConfigurationQuery, TerminalRegionalConfigurationDto>
    {
        private readonly ITerminalRepository _db;
        private readonly IRegionalService _regionalService;

        public GetTerminalRegionalConfigurationQueryHandler(ITerminalRepository db, IRegionalService regionalService)
        {
            _db = db;
            _regionalService = regionalService;
        }

        public async Task<TerminalRegionalConfigurationDto> Handle(GetTerminalRegionalConfigurationQuery request, CancellationToken cancellationToken)
        {
            var dto = new TerminalRegionalConfigurationDto();
            var config = await _db.GetCurrentRegionalConfigurationAsync()
                ?? new();

            //dto.PasswordStrongChecking = config.PasswordStrongChecking;
            dto.DaysLogFilesValid = config.DaysLogFilesValid;
            dto.MediumRedemption = config.MediumRedemption;
            dto.ValueFormat = config.ValueFormat;
            dto.CustomerNumber = config.CustomerNumber;
            dto.PropertyNumber = config.PropertyNumber;
            dto.AutomaticBackup = config.AutomaticBackup;
            dto.AutomaticLogout = config.AutomaticLogout;
            dto.BackupStartHour = config.BackupStartHour;
            dto.CasinoDayStarts = config.CasinoDayStarts;
            dto.Time = config.Time;
            dto.Distribution = config.Distribution;
            dto.SmallerNotes = config.SmallerNotes;
            dto.PasswordLength = config.PasswordLength;
            dto.PasswordValid = config.PasswordValid;
            dto.LocationId = config.LocationId;
            dto.LogEvents = config.LogEvents;

            dto.Caption = config.Caption;
            dto.MachineName = config.MachineName;
            dto.LocationAddress = config.LocationAddress;
            dto.LocationName = config.LocationName;

            dto.LocalTimeZone = config.LocalTimeZone;
            dto.LocalTimeZoneOptions.Add(new SelectListItem { Name = "None", Value = string.Empty });
            dto.LocalTimeZoneOptions.AddRange(
                _regionalService.GetAllTimeZones()
                .Select(x => new SelectListItem
                {
                    Name = x.DisplayName,
                    Value = x.Id,
                })
                .ToList());

            dto.DecimalSeparator = config.DecimalSeparator?.Code;
            dto.ThousandSeparator = config.ThousandSeparator?.Code;
            dto.DecimalSeparatorSymbol = config.DecimalSeparator?.Symbol;
            dto.ThousandSeparatorSymbol = config.ThousandSeparator?.Symbol;

            dto.AmountPrecision = config.AmountPrecision;

            dto.DateFormat = config.DateFormat.Code;
            dto.DateFormatOptions = new List<DateFormat>([DateFormat.DotDMY, DateFormat.SlashYMD, DateFormat.DashMDY])
                .Select(x => new SelectListItem()
                {
                    Name = x.Code,
                    Value = x.Code
                }).ToList();

            dto.DefaultLanguage = config.DefaultLanguage;
            dto.DefaultLanguageOptions = dto.DefaultLanguageOptions.Select(x => new SelectListItem()
            {
                Name = _regionalService.Translate(x.Name),
                Value = x.Value
            }).ToList();


            return await Task.FromResult(dto);
        }
    }
}
