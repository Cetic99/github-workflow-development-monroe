using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

/// <summary>
/// Represents a basic hardware device.
/// </summary>
public interface IBasicHardwareDevice : IDisposable
{
    /// <summary>
    /// Fully qualified name of the hardware device.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Application oparting mode
    /// </summary>
    TerminalOperatingMode Mode { get; }

    /// <summary>
    /// Sets application opearting mode
    /// </summary>
    void SetOperatingMode(TerminalOperatingMode mode);

    /// <summary>
    /// Initializes the hardware device.
    /// </summary>
    /// <returns>True if the initialization is successful, otherwise false.</returns>
    Task<bool> InitializeAsync();

    /// <summary>
    /// Enables the hardware device.
    /// </summary>
    /// <returns>True if the enabling is successful, otherwise false.</returns>
    Task<bool> EnableAsync();

    /// <summary>
    /// Disables the hardware device.
    /// </summary>
    /// <returns>True if the disabling is successful, otherwise false.</returns>
    Task<bool> DisableAsync();

    /// <summary>
    /// Resets the hardware device.
    /// </summary>
    /// <returns>True if reset is successful, otherwise false.</returns>
    Task<bool> ResetAsync();

    /// <summary>
    /// Resets the hardware device with the specified configuration.
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    Task<bool> ResetAsync(IBasicHardwareDeviceConfiguration configuration);

    /// <summary>
    /// Return device status.
    /// </summary>
    /// <returns>Device status in form of string.</returns>
    Task<string> GetCurrentStatus();

    /// <summary>
    /// Returns device warning text.
    /// </summary>
    /// <returns>Device warning in form of string.</returns>
    string GetWarning();

    /// <summary>
    /// Returns device error text.
    /// </summary>
    /// <returns>Device error in form of string.</returns>
    string GetError();

    /// <summary>
    /// Returns device information.
    /// </summary>
    /// <returns>Device info in form of string.</returns>
    string GetAdditionalDeviceInfo();

    /// <summary>
    /// Run device diagnostic command.
    /// </summary>
    /// <returns></returns>
    Task<OperationResult> RunDiagnosticsCommand(DeviceDiagnosticsCommand command, params object[] args);

    /// <summary>
    /// Gets a value indicating whether the hardware device is initialized.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Indicates whether a command is currently in progress (Reset, Enable, Disable...)
    /// </summary>
    bool CommandInProgress { get; }

    /// <summary>
    /// Supported device diagnostics commands.
    /// </summary>
    public IEnumerable<DeviceDiagnosticsCommand> SupportedDiagnosticCommands { get; }

    /// <summary>
    /// Gets a value indicating whether the hardware device is connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets a value indicating whether the hardware device is enabled and ready for commands.
    /// Device is active if it is enabled and connected.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether the hardware device is active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Event raised when the hardware device is disabled.
    /// </summary>
    event EventHandler DeviceDisabled;

    /// <summary>
    /// Event raised when the hardware device is enabled.
    /// </summary>
    event EventHandler DeviceEnabled;

    /// <summary>
    /// Event raised when the hardware device is disconnected.
    /// </summary>
    event EventHandler DeviceDisconnected;

    /// <summary>
    /// Event raised when error occurs with hardware device. Description of the error is provided.
    /// </summary>
    event EventHandler<string> ErrorOccured;

    /// <summary>
    /// Event raised when warning occurs with hardware device. Description of the warning is provided.
    /// </summary>
    event EventHandler<string> WarningRaised;
}
