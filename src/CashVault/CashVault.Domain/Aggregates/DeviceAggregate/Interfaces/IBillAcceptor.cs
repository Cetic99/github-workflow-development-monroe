using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

/// <summary>
/// Represents a bill acceptor interface.
/// </summary>
public interface IBillAcceptor : IBasicHardwareDevice
{
    /// <summary>
    /// Event raised when bill accepting is started by the bill acceptor.
    /// </summary>
    event EventHandler BillAcceptingStarted;

    /// <summary>
    /// Event raised when bill is rejected by the bill acceptor. The event argument is the reason for the rejection.
    /// </summary>
    event EventHandler<string> BillRejected;

    /// <summary>
    /// Event raised when bill is accepted by the bill acceptor.
    /// </summary>
    event EventHandler<decimal> BillAccepted;

    /// <summary>
    /// Event raised when ticket is rejected by the bill acceptor. The event argument is the reason for the rejection.
    /// </summary>
    event EventHandler<string> TicketRejected;

    /// <summary>
    /// Event raised when ticket is accepted by the bill acceptor.
    /// </summary>
    event EventHandler<string> TicketAccepted;

    /// <summary>
    /// Event raised when the bill acceptor cash box is removed from the device.
    /// </summary>
    event EventHandler StackBoxRemoved;

    /// <summary>
    /// Event raised when the bill acceptor cash box is full.
    /// </summary>
    event EventHandler StackBoxFull;

    /// <summary>
    /// Event raised when the bill jammed in acceptor.
    /// </summary>
    event EventHandler JamInAcceptor;

    /// <summary>
    /// Event raised when the bill jammed in stacker.
    /// </summary>
    event EventHandler JamInStacker;

    /// <summary>
    /// Gets the firmware version information of the bill acceptor.
    /// </summary>
    /// <returns></returns>
    public Task<string> GetFirmwareVersionAsync();
}
