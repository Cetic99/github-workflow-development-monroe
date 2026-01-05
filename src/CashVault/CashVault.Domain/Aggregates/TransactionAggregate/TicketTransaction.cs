using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.TransactionAggregate;

public class TicketTransaction : Transaction
{
    public int TicketId { get; private set; }
    private int? ticketTypeId { get; set; }
    private int? ticketTypeDetailId { get; set; }
    public Ticket? Ticket { get; private set; }
    public TicketTypeDetail? TicketTypeDetail { get; private set; }
    public TicketType? TicketType { get; private set; }

    private TicketTransaction() : base()
    {
    }

    public TicketTransaction(Ticket ticket,
                             TransactionType transactionType,
                             decimal previousCreditAmount,
                             TicketTypeDetail ticketTypeDetail,
                             string description = "Ticket transaction")
        : base(ticket.Amount,
               transactionType,
               description,
               previousCreditAmount,
               ticket.Barcode,
               ticket.Currency)
    {
        Ticket = ticket;
        TicketType = ticket.Type;
        TicketTypeDetail = ticketTypeDetail;
    }

    public TicketTransaction(decimal amountRequested,
                             TransactionType type,
                             TicketType ticketType,
                             decimal previousCreditAmount,
                             string externalReference,
                             string description = "Ticket transaction",
                             Currency? currency = null,
                             TicketTypeDetail? ticketTypeDetail = null)
        : base(amountRequested,
               type,
               description,
               previousCreditAmount,
               externalReference,
               currency)
    {
        TicketType = ticketType;
        TicketTypeDetail = ticketTypeDetail;
    }
}
