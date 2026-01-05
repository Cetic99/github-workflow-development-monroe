using CashVault.Application.Interfaces.Persistence;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using Microsoft.EntityFrameworkCore;

namespace CashVault.Infrastructure.PersistentStorage;

public class MoneyStatusRepository : IMoneyStatusRepository
{
    private readonly CashVaultContext _dbContext;

    public MoneyStatusRepository(CashVaultContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DispenserBillCountStatus> GetDispenserBillCountStatusAsync()
    {
        var item = await GetMoneyStatus<DispenserBillCountStatus>();

        if (item == null)
        {
            return new DispenserBillCountStatus();
        }

        return item as DispenserBillCountStatus;
    }

    public bool UpdateDispenserBillCountStatus(DispenserBillCountStatus billStatusCount)
    {
        UpdateMoneyStatus<DispenserBillCountStatus>(billStatusCount);

        return true;
    }

    public async Task<BillTicketAcceptorStackerStatus> GetBillTicketAcceptorBillCountStatusAsync()
    {
        var item = await GetMoneyStatus<BillTicketAcceptorStackerStatus>();

        if (item == null)
        {
            return new BillTicketAcceptorStackerStatus();
        }

        return item as BillTicketAcceptorStackerStatus;
    }

    public void UpdateBillTicketAcceptorBillCountStatus(BillTicketAcceptorStackerStatus stackerStatus)
    {
        UpdateMoneyStatus(stackerStatus);
    }

    public async Task<CoinAcceptorCollectorStatus> GetCoinAcceptorCollectorStatusAsync()
    {
        var item = await GetMoneyStatus<CoinAcceptorCollectorStatus>();

        if (item == null)
        {
            return new CoinAcceptorCollectorStatus();
        }

        return item as CoinAcceptorCollectorStatus ?? new();
    }

    public void UpdateCoinAcceptorCollectorStatus(CoinAcceptorCollectorStatus coinCollectorStatus)
    {
        UpdateMoneyStatus(coinCollectorStatus);
    }

    public async Task<CurrentCreditStatus> GetCurrentCreditStatusAsync()
    {
        var item = await GetMoneyStatus<CurrentCreditStatus>();

        if (item == null)
        {
            return new CurrentCreditStatus();
        }

        return item as CurrentCreditStatus;
    }

    public void UpdateCurrentCreditStatus(CurrentCreditStatus creditStatus)
    {
        UpdateMoneyStatus(creditStatus);
    }

    private async Task<MoneyStatus?> GetMoneyStatus<T>() where T : MoneyStatus
    {
        MoneyStatus? item = await _dbContext.MoneyStatuses.Where(x => x.Key == typeof(T).Name).FirstOrDefaultAsync() as T;
        item?.Initialize();

        return await Task.FromResult(item);
    }

    private void UpdateMoneyStatus<T>(T status) where T : MoneyStatus
    {
        status.ToJsonString();
        _dbContext.MoneyStatuses.Update(status);
    }

    public void AddMoneyStatusTransaction(MoneyStatusTransaction moneyStatusTransaction)
    {
        _dbContext.MoneyStatusTransactions.Add(moneyStatusTransaction);
    }
}