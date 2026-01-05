using CashVault.Domain.Aggregates.MessageAggregate;
using CashVault.Domain.Common;
using NodaTime;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class RegionalConfiguration
{
    // Default values for ticket printer template
    private string _caption = "CashVault";
    private string _locationName = "CashVault Office";
    private string _locationAddress = "Ive Lole Ribara bb / 78000 / Banja Luka";
    private string _machineName = "0001";
    private string? _defaultLanguage;
    private int _amountPrecision;

    public RegionalConfiguration()
    {
        if (string.IsNullOrEmpty(LocalTimeZone))
        {
            var tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalTimeZone = tz.Id;
        }
        DaysLogFilesValid = 7;
    }

    public int ValueFormat { get; set; }
    public string CasinoDayStarts { get; set; } = string.Empty;
    public string? LocalTimeZone { get; set; }
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
    public int MediumRedemption { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Caption { get => _caption; set => _caption = value; }
    public string LocationName { get => _locationName; set => _locationName = value; }
    public string LocationAddress { get => _locationAddress; set => _locationAddress = value; }
    public string MachineName { get => _machineName; set => _machineName = value; }
    public NumberSeparator? DecimalSeparator { get; set; }
    public NumberSeparator? ThousandSeparator { get; set; }
    public DateFormat DateFormat { get; set; } = DateFormat.DefaultDateFormat;
    public string? DefaultLanguage
    {
        get
        {
            if (string.IsNullOrEmpty(_defaultLanguage))
            {
                return Language.Default.Code;
            }
            return _defaultLanguage;
        }
        set
        {
            if (string.IsNullOrEmpty(value) || !Enumeration.Contains<Language>(value))
            {
                _defaultLanguage = Language.Default.Code;
            }
            else
            {
                _defaultLanguage = value;
            }
        }
    }
    public int AmountPrecision
    {
        get
        {
            if (_amountPrecision == 0)
            {
                return 2;
            }
            return _amountPrecision;
        }
        set
        {
            if (value < 2 || value > 8)
            {
                _amountPrecision = 2;
            }
            else
            {
                _amountPrecision = value;
            }
        }
    }
}
