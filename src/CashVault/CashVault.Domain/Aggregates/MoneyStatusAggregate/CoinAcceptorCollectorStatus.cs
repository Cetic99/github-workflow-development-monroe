using CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate;

public class CoinAcceptorCollectorStatus : MoneyStatus, ICloneable
{
    public int CoinCount { get; private set; }
    public decimal CoinAmount { get; private set; }

    public CoinAcceptorCollectorStatus()
    {
        CoinCount = 0;
        CoinAmount = 0;
    }

    [JsonConstructor]
    public CoinAcceptorCollectorStatus(
        int coinCount,
        decimal coinAmount)
    {
        CoinCount = coinCount;
        CoinAmount = coinAmount;
    }

    public void AddCoin(int count, decimal amount)
    {
        var oldStatus = (CoinAcceptorCollectorStatus)this.Clone();

        CoinCount += count;
        CoinAmount += amount;

        var newStatus = (CoinAcceptorCollectorStatus)this.Clone();

        AddDomainEvent(new CoinAddedToAcceptorCollectorEvent(oldStatus, newStatus));
    }

    public void Empty()
    {
        var oldStatus = (CoinAcceptorCollectorStatus)this.Clone();

        CoinCount = 0;
        CoinAmount = 0;

        var newStatus = (CoinAcceptorCollectorStatus)this.Clone();

        AddDomainEvent(new CoinAcceptorEmptiedEvent(oldStatus, newStatus));
    }

    public override void ToJsonString()
    {
        JsonValue = JsonSerializer.Serialize(this);
    }

    public override void Initialize()
    {
        if (JsonValue == null) return;

        CoinAcceptorCollectorStatus? obj = JsonSerializer.Deserialize<CoinAcceptorCollectorStatus>(JsonValue);

        if (obj != null)
        {
            CoinCount = obj.CoinCount;
            CoinAmount = obj.CoinAmount;
        }
    }

    public object Clone()
    {
        var json = JsonSerializer.Serialize(this);

        return JsonSerializer.Deserialize<CoinAcceptorCollectorStatus>(json)
            ?? new();
    }
}
