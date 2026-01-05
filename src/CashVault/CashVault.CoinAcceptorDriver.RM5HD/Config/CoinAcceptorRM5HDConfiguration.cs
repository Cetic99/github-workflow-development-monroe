using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using CashVault.Domain.ValueObjects;

namespace CashVault.CoinAcceptorDriver.RM5HD.Config;

public class CoinAcceptorRM5HDConfiguration : ICoinAcceptorConfiguration
{
    private List<CurrencyCoinDenomination>? _supportedCurrencies;

    public bool IsEnabled { get; set; }
    public List<SingleAcceptorCoinDenomination>? CoinDenominationConfig { get; set; }
    public Currency CurrentCurrency { get; set; } = Currency.Default;
    public List<CurrencyCoinDenomination> SupportedCurrencies
    {
        get
        {
            if (_supportedCurrencies != null)
                return _supportedCurrencies;

            var bamCoinDenominationConfig = new List<SingleAcceptorCoinDenomination>
            {
                new(Currency.BAM, 6, 5m),
                new(Currency.BAM, 5, 2m),
                new(Currency.BAM, 4, 1m),
                new(Currency.BAM, 3, 0.5m),
                new(Currency.BAM, 2, 0.2m),
                new(Currency.BAM, 1, 0.1m),
            };

            return _supportedCurrencies =
            [
                new CurrencyCoinDenomination(Currency.BAM, bamCoinDenominationConfig)
            ];
        }
        set => _supportedCurrencies = value;
    }

    public CoinAcceptorRM5HDConfiguration()
    {
        CurrentCurrency = Currency.Default;
        IsEnabled = false;
        CoinDenominationConfig = SupportedCurrencies
            .FirstOrDefault(c => c.Currency.Code == CurrentCurrency.Code)?
            .CoinDenominations;
    }

    public void Validate()
    {
        if (CoinDenominationConfig == null || CoinDenominationConfig.Count == 0)
        {
            throw new ArgumentNullException("Coin acceptor is not properly set");
        }
    }
}
