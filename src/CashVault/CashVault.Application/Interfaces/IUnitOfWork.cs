using CashVault.Application.Interfaces.Persistence;

namespace CashVault.Application.Interfaces;

public interface IUnitOfWork
{
    IMessageRepository MessageRepository { get; }
    ITerminalRepository TerminalRepository { get; }
    ITransactionRepository TransactionRepository { get; }
    ITicketRepository TicketRepository { get; }
    IOperatorRepository OperatorRepository { get; }
    IMoneyStatusRepository MoneyStatusRepository { get; }
    IEventLogRepository EventLogRepository { get; }
    IParcelLockerRepository ParcelLockerRepository { get; }

    Task<bool> SaveChangesAsync();
    bool SaveChanges();
}
