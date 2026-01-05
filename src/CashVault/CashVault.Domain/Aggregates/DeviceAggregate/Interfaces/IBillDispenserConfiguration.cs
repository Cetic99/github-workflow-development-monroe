using System.Collections.Generic;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

public interface IBillDispenserConfiguration : IBasicHardwareDeviceConfiguration
{
    /// <summary>
    /// Will dispenser raise low level warning when the number of bills in the cassette is low
    /// </summary>
    public bool SendLowLevelWarning { get; }
    /// <summary>
    /// List of bill cassettes that are configured in the dispenser
    /// </summary>
    public List<BillCasseteConfiguration> BillCassettes { get; }

    /// <summary>
    ///  If 'max number of count reject' is exceeded during counting, terminate counting and send negative response after rejecting the error bill.
    ///   If 'max number of count reject' "0" is specified, terminate without any retry upon detecting any count error. 
    /// </summary>
    public int MaxNumberOfCountReject { get; }
    /// <summary>
    /// Number of retries on error is 'Pick retries of a count’ (per each cassette) for pick failures and max number of count reject’ (per each cassette) for count errors.
    /// </summary>
    public int PickRetriesOfCount { get; }

    public int MaxBillCassettes { get; }

    /// <summary>
    /// Current configured currency
    /// </summary>
    public Currency CurrentCurrency { get; }
    public List<Currency> SupportedCurrencies { get; }

    public List<BillCasseteConfiguration> GetBillCassettes();
}
