using CashVault.Domain.Aggregates.MoneyStatusAggregate;

namespace CashVault.Application.Interfaces.Persistence;

public interface IMoneyStatusRepository
{
    Task<DispenserBillCountStatus> GetDispenserBillCountStatusAsync();
    bool UpdateDispenserBillCountStatus(DispenserBillCountStatus billStatusCount);

    Task<BillTicketAcceptorStackerStatus> GetBillTicketAcceptorBillCountStatusAsync();
    void UpdateBillTicketAcceptorBillCountStatus(BillTicketAcceptorStackerStatus stackerStatus);

    Task<CoinAcceptorCollectorStatus> GetCoinAcceptorCollectorStatusAsync();
    void UpdateCoinAcceptorCollectorStatus(CoinAcceptorCollectorStatus coinCollectorStatus);

    Task<CurrentCreditStatus> GetCurrentCreditStatusAsync();
    void UpdateCurrentCreditStatus(CurrentCreditStatus creditStatus);

    void AddMoneyStatusTransaction(MoneyStatusTransaction moneyStatusTransaction);
}