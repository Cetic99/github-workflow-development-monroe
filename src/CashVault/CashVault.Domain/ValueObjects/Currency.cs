using System;
using System.Collections.Generic;
using CashVault.Domain.Common;

namespace CashVault.Domain.ValueObjects;

public partial class Currency : Enumeration
{
    public enum CurrencySymbolPositionEnum
    {
        BeforeValue,
        AfterValue
    }

    public string Symbol { get; private set; }
    public CurrencySymbolPositionEnum SymbolPosition { get; private set; }

    public Currency(int id, string code, string symbol, CurrencySymbolPositionEnum symbolPosition = CurrencySymbolPositionEnum.AfterValue) : base(id, code)
    {
        Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        SymbolPosition = symbolPosition;
    }

    public string GetFullCurrencyName()
    {
        return $"{Code} ({Symbol})";
    }

    public string GetFormattedValue(decimal value)
    {
        return SymbolPosition switch
        {
            CurrencySymbolPositionEnum.BeforeValue => $"{Symbol} {value}",
            CurrencySymbolPositionEnum.AfterValue => $"{value} {Symbol}",
            _ => throw new InvalidOperationException()
        };
    }
}

/// <summary>
/// Currencies default values with denom keys, symbol position and supported denominations
/// </summary>
/// <remarks>
/// TODO: Add denomKey values for coins and add denom keys for EUR AND USD!!
/// </remarks>
public partial class Currency
{
    public static readonly Currency BAM = new(
        id: 1,
        code: "BAM",
        symbol: "KM",
        symbolPosition: CurrencySymbolPositionEnum.AfterValue
        );

    public static readonly Currency USD = new(
        id: 2,
        code: "USD",
        symbol: "$",
        symbolPosition: CurrencySymbolPositionEnum.BeforeValue
    );

    public static readonly Currency EUR = new(
        id: 3,
        code: "EUR",
        symbol: "€",
        symbolPosition: CurrencySymbolPositionEnum.AfterValue
    );

    public static readonly Currency Default = BAM;
}