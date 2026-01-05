using CashVault.Application.Features.TicketFeatures.Queries;

namespace CashVault.Application.Interfaces;

public interface ITicketProvider
{
    /// <summary>
    /// Redeem a ticket by its barcode.
    /// </summary>
    /// <param name="barcode">Ticket barcode</param>
    /// <param name="id">Optional identifier for the ticket redemption request</param>
    Task<RedeemTicketDto?> RedeemTicket(string barcode, Guid? id);

    /// <summary>
    /// Send ACK for a redeemed ticket.
    /// </summary>
    /// <param name="barcode">Ticket barcode</param>
    /// <param name="id">Optional identifier for the ticket redemption request</param>
    Task<bool> RedeemTicketAck(string barcode, Guid? id);

    /// <summary>
    /// Send NACK for a ticket.
    /// </summary>
    /// <param name="barcode">Ticket barcode</param>
    /// <param name="id">Optional identifier for the ticket redemption request</param>
    Task<bool> RedeemTicketNack(string barcode, Guid? id);
}
