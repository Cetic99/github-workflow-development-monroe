using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.TicketAggregate;

public class TicketType : Enumeration
{
    public static TicketType TITO = new(1, nameof(TITO).ToLowerInvariant());
    public static TicketType MonroeCashConfirmation = new(2, nameof(MonroeCashConfirmation).ToLowerInvariant());
    public static TicketType BetboxTicket = new(3, nameof(BetboxTicket).ToLowerInvariant());

    public TicketType(int id, string code) : base(id, code)
    {
    }
}
