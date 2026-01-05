using System.Threading.Tasks;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

/// <summary>
/// Interface for Parcel Locker device driver
/// </summary>
public interface IParcelLocker : IBasicHardwareDevice
{
    /// <summary>
    /// Opens a specific cabinet door
    /// </summary>
    /// <param name="cabinetNumber">Cabinet number </param>
    /// <returns>True if operation successful, false otherwise</returns>
    Task<bool> OpenCabinetAsync(int cabinetNumber);

    /// <summary>
    /// Reads the door status of a specific cabinet
    /// </summary>
    /// <param name="cabinetNumber">Cabinet number </param>
    /// <returns>True if door is closed, false if open</returns>
    Task<bool> IsCabinetClosedAsync(int cabinetNumber);

    /// <summary>
    /// Reads the occupancy status of a specific cabinet
    /// </summary>
    /// <param name="cabinetNumber">Cabinet number </param>
    /// <returns>True if cabinet is occupied, false if unoccupied</returns>
    Task<bool> IsCabinetOccupiedAsync(int cabinetNumber);
}
