using CashVault.Application.Interfaces;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Infrastructure.TicketProviders.Betbox;
using Microsoft.Extensions.DependencyInjection;

namespace CashVault.Infrastructure.TicketProviders;

public class TicketProviderFactory : ITicketProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TicketProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITicketProvider GetTicketProviderByType(TicketType ticketType)
    {
        var providers = _serviceProvider.GetServices<ITicketProvider>();

        if (ticketType == TicketType.BetboxTicket)
        {
            var provider = providers.OfType<BetboxTicketProvider>().First();
            return provider;
        }

        throw new NotSupportedException($"Ticket provider for type {ticketType.Code} is not supported.");
    }
}
