namespace CashVault.Infrastructure.TicketProviders.Betbox;

internal enum BetboxTicketStatus
{
    Active = 0,
    Used = 1,
    Expired = 2,
    Reserved = 3,
    Unknown = 99
}
