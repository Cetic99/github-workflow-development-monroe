using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

/// <summary>
/// Represents a coin acceptor interface.
/// </summary>
public interface ICoinAcceptor : IBasicHardwareDevice
{
    /// <summary>
    /// Event raised when coin is rejected by the coin acceptor. The event argument is the reason for the rejection.
    /// </summary>
    event EventHandler<string> CoinRejected;

    /// <summary>
    /// Event raised when coin is accepted by the coin acceptor.
    /// </summary>
    event EventHandler<decimal> CoinAccepted;

    /// <summary>
    /// Gets the firmware version information of the coin acceptor.
    /// </summary>
    /// <returns></returns>
    public Task<string> GetFirmwareVersionAsync();
}
