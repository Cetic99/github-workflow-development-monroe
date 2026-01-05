using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.TicketAggregate;

public class TicketTypeDetail : Enumeration
{
    public static readonly TicketTypeDetail Cms = new(1, nameof(Cms).ToLower());
    public static readonly TicketTypeDetail Local = new(2, nameof(Local).ToLower());

    public TicketTypeDetail(int id, string code) : base(id, code) { }
}
