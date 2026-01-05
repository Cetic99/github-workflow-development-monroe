using System;
using System.Threading.Tasks;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

/// <summary>
/// Represents a ticket printer interface.
/// </summary>
public interface ITITOPrinter : IBasicHardwareDevice
{
    /// <summary>
    /// Event raised when ticket printing is started.
    /// </summary>
    event EventHandler TicketPrintingStarted;

    /// <summary>
    /// Event raised when ticket printing is completed.
    /// </summary>
    event EventHandler TicketPrintingCompleted;

    /// <summary>
    /// Event raised when ticket printing fails.
    /// </summary>
    event EventHandler TicketPrintingFailed;

    /// <summary>
    /// Prints a ticket using the terminal's printer with the specified details from CMS.
    /// </summary>
    /// <param name="ticket">The ticket object containing barcode, amount, currency, and validity information.</param>
    /// <param name="caption">The title or caption to be printed on the ticket.</param>
    /// <param name="locationName">The name of the location where the ticket is printed.</param>
    /// <param name="locationAddress">The address of the location where the ticket is printed.</param>
    /// <param name="daysValid">The number of days the ticket is valid.</param>
    /// <returns>True if the ticket is printed successfully, otherwise false.</returns>
    public Task<OperationResult> PrintTicketAsync(Ticket ticket, string caption, string locationName, string locationAddress, string machineNumber);

    /// <summary>
    /// Prints the specified text. Lines are separated by new line character.
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    Task<OperationResult> PrintTextAsync(string[] lines);
}
