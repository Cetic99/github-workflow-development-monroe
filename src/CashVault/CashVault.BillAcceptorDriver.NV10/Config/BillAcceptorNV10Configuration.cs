using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.Common;
using CashVault.Domain.ValueObjects;

namespace CashVault.BillAcceptorDriver.NV10.Config;

public class BillAcceptorNV10Configuration : IBillAcceptorConfiguration
{
    public List<SingleAcceptorBillDenomination>? BillDenominationConfig { get; set; } = [];

    public bool IsEnabled { get; set; }

    public Currency CurrentCurrency { get; set; }

    public bool AcceptTITOTickets { get; set; }

    public BillAcceptorNV10Configuration()
    {
        IsEnabled = true;
        //Initialize all supported paper bills
        CurrentCurrency = Enumeration.GetByCode<Currency>(Currency.Default.Code);
        BillDenominationConfig =
            SupportedCurrencies
                .Where(c => c.currency.Code == Currency.Default.Code)
                .First()?.BillDenominations
                .Select(d => new SingleAcceptorBillDenomination(d.Currency, d.DataKey, d.DataValue, true))
                .ToList();
        AcceptTITOTickets = true;
    }

    /// <summary>
    /// Method used to obtain which currencies are supported by the bill acceptor.
    /// </summary>
    /// <returns></returns>
    public List<BillAcceptorCurrencyBillDenomination> SupportedCurrencies { get; } = new()
    {
        new BillAcceptorCurrencyBillDenomination(
            Currency.BAM,
            new List<SingleAcceptorBillDenomination>
            {
                new(Currency.BAM, 5, 200),
                new(Currency.BAM, 4, 100),
                new(Currency.BAM, 3, 50),
                new(Currency.BAM, 2, 20),
                new(Currency.BAM, 1, 10),
            }
        ),

        // TODO: Add more supported currencies
    };

    public void Validate()
    {
        if (BillDenominationConfig == null)
        {
            throw new ArgumentNullException("BillDenominationConfig is null");
        }

        foreach (var billDenomConfig in BillDenominationConfig)
        {
            if (billDenomConfig == null || billDenomConfig.DataValue == default || billDenomConfig.DataKey == default || billDenomConfig.Currency == null)
            {
                throw new InvalidOperationException("Bill Acceptor denomination config is not properly set");
            }
        }
    }
}
