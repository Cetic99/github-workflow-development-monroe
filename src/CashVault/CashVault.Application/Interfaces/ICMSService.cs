using CashVault.Application.Common.Models.CMSResponses;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Common.Events;

namespace CashVault.Application.Interfaces
{
    public interface ICMSService
    {
        /// <summary>
        /// Redeems a ticket using the provided barcode.
        /// </summary>
        /// <param name="barcode">The barcode of the ticket to redeem.</param>
        /// <returns>A <see cref="TicketRedemptionResponseModel"/> indicating the redemption status, or null if the CMS system is not available.</returns>
        public Task<TicketRedemptionResponseModel?> RedeemTicket(string barcode);

        /// <summary>
        /// Completes the ticket redemption process using the provided barcode.
        /// </summary>
        /// <param name="barcode">The barcode of the ticket to complete the redemption for.</param>
        /// <returns>A boolean indicating whether the redemption was completed successfully.</returns>
        public Task<bool> CompleteRedeemTicket(string barcode);

        /// <summary>
        /// Handles a failed ticket redemption process using the provided barcode.
        /// </summary>
        /// <param name="barcode">The barcode of the ticket to fail the redemption for.</param>
        /// <returns>A boolean indicating whether the failed redemption was processed successfully.</returns>
        public Task<bool> FailTicketRedemption(string barcode);

        /// <summary>
        /// Requests ticket printing with the given amount.
        /// </summary>
        /// <param name="amount">The amount to be printed on the ticket.</param>
        /// <returns>A <see cref="TicketPrintResponseModel"/> indicating the printing request status, or null if the CMS system is not available.</returns>
        public Task<TicketPrintResponseModel?> RequestTicketPrinting(decimal amount);

        /// <summary>
        /// Completes the ticket printing process using the provided barcode.
        /// </summary>
        /// <param name="barcode">The barcode of the ticket to complete the printing for.</param>
        /// <returns>A boolean indicating whether the ticket printing was completed successfully.</returns>
        public Task<bool> CompleteTicketPrinting(string barcode);

        /// <summary>
        /// Handles a failed ticket printing process using the provided barcode.
        /// </summary>
        /// <param name="barcode">The barcode of the ticket to fail the printing for.</param>
        /// <returns>A boolean indicating whether the failed printing was processed successfully.</returns>
        public Task<bool> FailTicketPrinting(string barcode);

        /// <summary>
        /// Sends an event to the CMS system.
        /// </summary>
        /// <param name="event">The event to be sent to the CMS system.</param>
        /// <returns>A boolean indicating whether the event was sent successfully.</returns>
        public Task<bool> SendEvent(BaseEvent @event);

        /// <summary>
        /// Sends a transaction to the CMS system.
        /// </summary>
        /// <param name="transactionEvent">The transaction event to be sent to the CMS system.</param>
        /// <returns>A boolean indicating whether the transaction was sent successfully.</returns>
        public Task<bool> SendTransactionEvent(TransactionEvent transactionEvent, DispenserBillCountStatus dispenserBillCountStatus, BillTicketAcceptorStackerStatus acceptorMoneyStatus);
    }
}
