using System.Threading.Tasks;
using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

/// <summary>
/// Represents a bill dispenser hardware interface.
/// </summary>
public interface IBillDispenser : IBasicHardwareDevice
{
    /// <summary>
    /// Dispenses cash to the reject tray as it is defined in transaction which is prepared by the caller.
    /// </summary>
    /// <param name="transaction">Prepared dispenser bill transaction. Depending on dispensing results, transaction properties are updated</param>
    /// <returns>False if device can't process reject request at all, true otherwise.</returns>
    Task<bool> RejectCash(DispenserBillTransaction transaction);

    /// <summary>
    /// Dispenses the transaction which is prepared by the caller.
    /// </summary>
    /// <param name="transaction">Prepared dispenser bill transaction. Depending on dispensing results, transaction properties are updated</param>
    /// <returns>IsSuccess is true if transaction is completed successfully, otherwise false. Error string contains errors description if exists.</returns>

    Task<OperationResult> DispenseCashAsync(DispenserBillTransaction transaction);

}
