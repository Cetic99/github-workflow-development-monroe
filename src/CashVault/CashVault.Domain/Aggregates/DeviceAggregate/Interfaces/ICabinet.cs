using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

/// <summary>
/// Represents a cabinet interface.
/// </summary>
public interface ICabinet : IBasicHardwareDevice
{
    /// <summary>
    /// Event raised when the cabinet door is opened.
    /// return: Id of sensor
    /// </summary>
    event EventHandler<int> DoorOpened;

    /// <summary>
    /// Event raised when the cabinet door is closed.
    /// /// return: Id of sensor
    /// </summary>
    event EventHandler<int> DoorClosed;

    /// <summary>
    /// Event raised when vibration is detected in the cabinet.
    /// </summary>
    event EventHandler VibrationDetected;

    /// <summary>
    /// Send command for checking all door sensors
    /// </summary>
    /// <returns> bool </returns>
    public Task<bool> CheckAllDoorSensorsStatuses();

    /// <summary>
    /// Send command for checking specific door sensors
    /// </summary>
    /// /// <param name="sensorId">ID of specific door sensor</param>
    /// <returns> bool </returns>
    public Task<bool> CheckSpecificDoorSensorStatus(int sensorId);

}
