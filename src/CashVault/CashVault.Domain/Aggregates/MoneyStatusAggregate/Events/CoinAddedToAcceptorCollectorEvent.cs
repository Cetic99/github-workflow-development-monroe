using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;

public class CoinAddedToAcceptorCollectorEvent : TransactionEvent
{
    public CoinAcceptorCollectorStatus OldCoinCollectorStatus { get; private set; }
    public CoinAcceptorCollectorStatus NewCoinCollectorStatus { get; private set; }

    public CoinAddedToAcceptorCollectorEvent(
        CoinAcceptorCollectorStatus oldCoinCollectorStatus,
        CoinAcceptorCollectorStatus newCoinCollectorStatus)
        : base($"Coin added to acceptor collector.")
    {
        OldCoinCollectorStatus = oldCoinCollectorStatus;
        NewCoinCollectorStatus = newCoinCollectorStatus;
    }
}
