using CashVault.Application.Common.Models;
using CashVault.Domain.Aggregates.MessageAggregate;
using CashVault.Domain.Common;

namespace CashVault.Application.Features.DeviceFeatures.Queries
{
    public class TerminalRegionalConfigurationDto
    {
        public int ValueFormat { get; set; }
        public List<SelectListItem> ValueFormatOptions { get; set; }
        public string? CasinoDayStarts { get; set; }
        public string? LocalTimeZone { get; set; }
        public List<SelectListItem> LocalTimeZoneOptions { get; set; }
        public int PasswordLength { get; set; }
        public bool PasswordStrongChecking { get; set; }
        public int PasswordValid { get; set; }
        public int AutomaticLogout { get; set; }
        public int DaysLogFilesValid { get; set; }
        public bool LogEvents { get; set; }
        public bool AutomaticBackup { get; set; }
        public int BackupStartHour { get; set; }
        public int LocationId { get; set; }
        public int PropertyNumber { get; set; }
        public int CustomerNumber { get; set; }
        public int Distribution { get; set; }
        public int SmallerNotes { get; set; }
        public List<SelectListItem> SmallerNotesOptions { get; set; }
        public int MediumRedemption { get; set; }
        public List<SelectListItem> MediumRedemptionOptions { get; set; }
        public string? Time { get; set; }
        public string Caption { get; set; } = null!;
        public string LocationName { get; set; } = null!;
        public string LocationAddress { get; set; } = null!;
        public string MachineName { get; set; } = null!;
        public string? DecimalSeparator { get; set; }
        public string? ThousandSeparator { get; set; }
        public string? DecimalSeparatorSymbol { get; set; }
        public string? ThousandSeparatorSymbol { get; set; }
        public List<SelectListItem> NumberSeparatorOptions { get; set; }
        public string? DefaultLanguage { get; set; }
        public List<SelectListItem> DefaultLanguageOptions { get; set; }
        public int AmountPrecision { get; set; }
        public string? DateFormat { get; set; }
        public List<SelectListItem> DateFormatOptions { get; set; }

        public TerminalRegionalConfigurationDto()
        {
            ValueFormatOptions = [];
            LocalTimeZoneOptions = [];
            SmallerNotesOptions = [];
            MediumRedemptionOptions = [];
            NumberSeparatorOptions = [];
            DefaultLanguageOptions = [];
            DateFormatOptions = [];

            DefaultLanguageOptions.Add(new SelectListItem()
            {
                Name = nameof(Language.English),
                Value = Language.English.Code
            });

            DefaultLanguageOptions.Add(new SelectListItem()
            {
                Name = nameof(Language.Serbian),
                Value = Language.Serbian.Code
            });

            ValueFormat = 0;
            ValueFormatOptions.Add(new SelectListItem()
            {
                Name = "None",
                Value = "---"
            });
            ValueFormatOptions.Add(new SelectListItem()
            {
                Name = "French (Canada)",
                Value = "fr-CA"
            });

            LocalTimeZone = null;

            SmallerNotes = 0;
            SmallerNotesOptions.Add(new SelectListItem()
            {
                Name = "None",
                Value = "---"
            });
            SmallerNotesOptions.Add(new SelectListItem()
            {
                Name = "Largest bills possible",
                Value = "largest"
            });

            MediumRedemption = 0;
            MediumRedemptionOptions.Add(new SelectListItem()
            {
                Name = "None",
                Value = "---"
            });
            MediumRedemptionOptions.Add(new SelectListItem()
            {
                Name = "Largest bills possible",
                Value = "largest"
            });

            NumberSeparatorOptions.Add(new SelectListItem()
            {
                Name = "None",
                Value = null
            });
            NumberSeparatorOptions.Add(
                new SelectListItem()
                {
                    Name = NumberSeparator.Dot.Symbol,
                    Value = NumberSeparator.Dot.Code
                });

            NumberSeparatorOptions.Add(
            new SelectListItem()
            {
                Name = NumberSeparator.Comma.Symbol,
                Value = NumberSeparator.Comma.Code
            });
        }
    }
}