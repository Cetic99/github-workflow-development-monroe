using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.ValueObjects;

namespace CashVault.BillDispenserDriver.JCM.F53.Config;

public class BillDispenserJcm53Configuration : IBillDispenserConfiguration
{
    /// <summary>
    /// Will dispenser raise low level warning when the number of bills in the cassette is low
    /// </summary>
    public bool SendLowLevelWarning { get; set; }
    /// <summary>
    /// List of bill cassettes that are configured in the dispenser
    /// </summary>
    public List<BillCasseteConfiguration> BillCassettes { get; set; }
    public bool IsEnabled { get; set; }

    /// <summary>
    ///  If 'max number of count reject' is exceeded during counting, terminate counting and send negative response after rejecting the error bill.
    ///   If 'max number of count reject' "0" is specified, terminate without any retry upon detecting any count error. 
    /// </summary>
    public int MaxNumberOfCountReject { get; set; }
    /// <summary>
    /// Number of retries on error is 'Pick retries of a count’ (per each cassette) for pick failures and max number of count reject’ (per each cassette) for count errors.
    /// </summary>
    public int PickRetriesOfCount { get; set; }

    public int MaxBillCassettes { get; private set; } = 6;

    /// <summary>
    /// Current configured currency
    /// </summary>
    public Currency CurrentCurrency { get; set; }

    public List<Currency> SupportedCurrencies { get; private set; } =
    [
        Currency.Default,
    ];

    public BillDispenserJcm53Configuration()
    {
        List<BillDenomination> billDenominations = SupportedDenominations.Where(x => x.Currency.Code == Currency.Default.Code).ToList();
        List<BillCasseteConfiguration> cassetteConfig =
                [
                    new BillCasseteConfiguration(1,billDenominations.ElementAt(3)),
                    new BillCasseteConfiguration(3,billDenominations.ElementAt(2)),
                    new BillCasseteConfiguration(4,billDenominations.ElementAt(1)),
                    new BillCasseteConfiguration(2,billDenominations.ElementAt(0)),
                ];

        this.SendLowLevelWarning = false;
        this.IsEnabled = false;
        this.CurrentCurrency = Currency.Default;
        this.BillCassettes = cassetteConfig;
        this.MaxNumberOfCountReject = 3;
        this.PickRetriesOfCount = 3;
    }

    public BillDispenserJcm53Configuration(bool sendLowLevelWarning, bool isEnabled, Currency currentCurrency, List<BillCasseteConfiguration>? billCassettes = null, int maxNumberOfCountReject = 3, int pickRetriesOfCount = 3)
    {
        this.SendLowLevelWarning = sendLowLevelWarning;
        this.IsEnabled = isEnabled;
        this.BillCassettes = billCassettes ?? [];
        this.CurrentCurrency = currentCurrency;
        this.MaxNumberOfCountReject = maxNumberOfCountReject;
        this.PickRetriesOfCount = pickRetriesOfCount;
    }

    public void ClearBillCassettes()
    {
        BillCassettes.Clear();
    }

    public void SetBillCassetteConfiguration(BillCasseteConfiguration cassetteConfig)
    {
        var cassette = BillCassettes.FirstOrDefault(c => c.CassetteNumber == cassetteConfig.CassetteNumber);
        if (cassette == null)
        {
            BillCassettes.Add(cassetteConfig);
        }
        else
        {
            cassette.SetBillDenomination(cassetteConfig.BillDenomination);
            cassette.SetDenominationMagnetStatus(cassetteConfig.DenominationMagnetStatus);
        }
    }

    public void SetIsEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    public void SetSendLowLevelWarning(bool sendLowLevelWarning)
    {
        SendLowLevelWarning = sendLowLevelWarning;
    }

    public void SetMaxNumberOfCountReject(int maxNumberOfCountReject)
    {
        MaxNumberOfCountReject = maxNumberOfCountReject;
    }

    public void SetPickRetriesOfCount(int pickRetriesOfCount)
    {
        PickRetriesOfCount = pickRetriesOfCount;
    }

    public List<BillDenomination> SupportedDenominations { get; } =
     [
        new(100, Currency.BAM, 74, 154, 11),
        new(50, Currency.BAM, 71, 146, 11),
        new(20, Currency.BAM, 68, 138, 11),
        new(10, Currency.BAM, 65, 130, 11),
    ];

    public List<BillCasseteConfiguration> GetBillCassettes()
    {
        return BillCassettes ?? [];
    }

    public void Validate()
    {
        if (BillCassettes.Count == 0)
        {
            throw new ArgumentException("Bill dispenser configuration must have at least one bill cassette configured.");
        }

        if (BillCassettes.Count > MaxBillCassettes)
        {
            throw new ArgumentException($"Bill dispenser configuration can have at most {MaxBillCassettes} bill cassettes configured.");
        }

        if (BillCassettes.Count != BillCassettes.Select(c => c.CassetteNumber).Distinct().Count())
        {
            throw new ArgumentException("Bill cassette numbers must be unique.");
        }

        if (BillCassettes.Count != BillCassettes.Select(c => c.BillDenomination.Value).Distinct().Count())
        {
            throw new ArgumentException("Bill cassette denomination values must be unique.");
        }

        if (BillCassettes.Select(c => c.BillDenomination.Currency).Distinct().Count() > 1)
        {
            throw new ArgumentException("All bill cassettes must have the same currency.");
        }

        if (BillCassettes.Count != BillCassettes.Select(c => c.DenominationMagnetStatus?.GetHashCode()).Distinct().Count())
        {
            throw new ArgumentException("Bill cassette magnet statuses must be unique.");
        }

        if (MaxNumberOfCountReject > 20)
        {
            throw new ArgumentOutOfRangeException("MaxNumberOfCountReject", "MaxNumberOfCountReject must be less than or equal to 20.");
        }

        if (PickRetriesOfCount > 15)
        {
            throw new ArgumentOutOfRangeException("PickRetriesOfCount", "PickRetriesOfCount must be less than or equal to 15.");
        }
    }
}
