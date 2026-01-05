using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;

public class CoinAcceptorEmptiedEvent : TransactionEvent
{
    public CoinAcceptorCollectorStatus OldCoinCollectorStatus { get; private set; }
    public CoinAcceptorCollectorStatus NewCoinCollectorStatus { get; private set; }

    public CoinAcceptorEmptiedEvent(
        CoinAcceptorCollectorStatus oldCoinCollectorStatus,
        CoinAcceptorCollectorStatus newCoinCollectorStatus)
        : base($"Coin acceptor emptied.")
    {
        OldCoinCollectorStatus = oldCoinCollectorStatus;
        NewCoinCollectorStatus = newCoinCollectorStatus;
    }
}
