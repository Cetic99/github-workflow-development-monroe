using CashVault.Domain.Aggregates.TicketAggregate;

namespace CashVault.Application.Interfaces;

public interface ITicketProviderFactory
{
    /// <summary>
    /// Get ITicketProvider implementation by TicketType.
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    ITicketProvider GetTicketProviderByType(TicketType ticketType);
}
