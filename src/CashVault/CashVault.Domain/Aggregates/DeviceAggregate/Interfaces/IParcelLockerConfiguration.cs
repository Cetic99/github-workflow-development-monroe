using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using System;
using System.Collections.Generic;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

public interface IParcelLockerConfiguration : IBasicHardwareDeviceConfiguration
{
    /// <summary>
    /// Supported values: 9600, 14400, 19200, 38400, 57600, 115200
    /// </summary>
    List<int> SupportedBaudRates { get; set; }

    /// <summary>
    /// Board address (0x01 to 0x40, supports max 64 boards)
    /// </summary>
    byte BoardAddress { get; set; }

    /// <summary>
    /// Serial port baud rate
    /// </summary>
    int BaudRate { get; set; }

    /// <summary>
    /// Number of cabinets on the board
    /// </summary>
    int CabinetCount { get; set; }

    /// <summary>
    /// Enable periodic status checking
    /// </summary>
    bool EnablePeriodicStatusCheck { get; set; }

    /// <summary>
    /// Status check interval in seconds
    /// </summary>
    int StatusCheckIntervalSeconds { get; set; }

    /// <summary>
    /// Parcel lockers
    /// </summary>
    List<ParcelLocker> ParcelLockers { get; set; }

    ParcelLocker? GetLocker(int id);
    ParcelLocker? GetLocker(Func<ParcelLocker, bool> predicate);
    ParcelLocker? GetAvailableLocker(string postalService, string size);
}
