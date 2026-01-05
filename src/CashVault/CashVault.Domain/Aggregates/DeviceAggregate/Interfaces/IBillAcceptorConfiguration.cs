using System.Collections.Generic;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

public interface IBillAcceptorConfiguration : IBasicHardwareDeviceConfiguration
{
    /// <summary>
    /// List of paper denominations that the bill acceptor can accept.
    /// </summary>
    public List<SingleAcceptorBillDenomination>? BillDenominationConfig { get; set; }

    /// <summary>
    /// Current configured currency
    /// </summary>
    public Currency CurrentCurrency { get; set; }

    /// <summary>
    /// Is the bill acceptor configured to accept TITO tickets.
    /// </summary>
    public bool AcceptTITOTickets { get; set; }
}
