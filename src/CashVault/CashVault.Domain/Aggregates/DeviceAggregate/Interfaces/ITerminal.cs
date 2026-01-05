using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

/// <summary>
/// Interface that aggregates methods and events for interacting with the hardware
/// </summary>
public interface ITerminal : IAggregateRoot, IDisposable
{
    TerminalTypeDeviceConfiguration TerminalTypeDeviceConfiguration { get; }
    bool UserLoginEnabled { get; }
    bool BillDispenserEnabled { get; }
    bool BillAcceptorEnabled { get; }
    bool CardReaderEnabled { get; }
    bool TITOPrinterEnabled { get; }
    List<Device> Devices { get; }
    string? LocalTimeZone { get; }
    int AmountPrecision { get; }
    string TerminalStatus { get; }

    /// <summary>
    /// Attempts to start a transaction. If a transaction is already in progress, returns false.
    /// Ensures thread-safe access using a semaphore.
    /// </summary>
    Task<bool> StartTransaction();

    /// <summary>
    /// Ends the current transaction, allowing new transactions to be started.
    /// Ensures thread-safe access using a semaphore.
    /// </summary>
    Task EndTransaction();


    IBillDispenser? BillDispenser { get; }
    ITITOPrinter? TITOPrinter { get; }
    IUserCardReader? UserCardReader { get; }
    IBillAcceptor? BillAcceptor { get; }
    ICabinet? Cabinet { get; }
    ICoinAcceptor? CoinAcceptor { get; }
    IParcelLocker? ParcelLocker { get; }

    List<Port> GetAvailablePorts();
    bool IsPortAvailable(string portName);
    List<DeviceModel> GetAvailableDeviceModels();
    IBasicHardwareDevice? GetDeviceByType(DeviceType deviceType);

    /// <summary>
    /// Starts the device hub on system startup
    /// </summary>
    Task StartAsync();

    /// <summary>
    /// Resets the entire device hub. Usefull when the device hub is in an invalid state or crucial configuration update occurs that requires a reset of all connected devices.
    /// </summary>
    Task ResetAsync();

    /// <summary>
    /// Stops the device hub when the system is shutting down
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Resets a specific device. Usefull when a device is in an invalid state or crucial configuration update occurs that requires a reset of target device.
    /// </summary>
    /// <param name="device"></param>
    Task<bool> ResetDeviceAsync(DeviceType deviceType);

    /// <summary>
    /// Enables a specific device. Usefull when a device is disabled and needs to be enabled.
    /// </summary>
    /// <param name="deviceType">Actual device type to be enabled</param>
    /// <returns>True if enabled successfuly, otherwise false.</returns>
    Task<bool> EnableDeviceAsync(DeviceType deviceType);

    /// <summary>
    /// Disables a specific device. Usefull when a device is enabled and needs to be disabled.
    /// </summary>
    /// <param name="deviceType">True if enabled successfuly, otherwise false.</param>
    /// <returns></returns>
    Task<bool> DisableDeviceAsync(DeviceType deviceType);

    /// <summary>
    /// Gets all devices info.
    /// </summary>
    List<IBasicHardwareDevice> GetDevicesAsync();

    /// <summary>
    /// Sets the user login enabled state. This is used to enable or disable user login functionality on the terminal.
    /// Enable set UserLoginEnabled to true and set timer to default value, after timeout UserLoginEnabled is set to false automatically.
    /// </summary>
    /// <param name="enabled"></param>
    void SetUserLoginEnabled(bool enabled);

    /// <summary>
    /// Indicates whether a transaction is currently in progress.
    /// </summary>
    bool IsTransactionInProgress { get; }

    //==============================================================

    TerminalOperatingMode OperatingMode { get; }

    List<TerminalType> TerminalTypes { get; }

    List<Port> AvailablePorts { get; }

    List<DeviceModel> AvailableDevices { get; }

    IBillAcceptorConfiguration BillAcceptorConfiguration { get; }

    IBillDispenserConfiguration BillDispenserConfiguration { get; }

    ITITOPrinterConfiguration TITOPrinterConfiguration { get; }
    ICoinAcceptorConfiguration CoinAcceptorConfiguration { get; }

    IParcelLockerConfiguration ParcelLockerConfiguration { get; }

    NetworkConfiguration NetworkConfiguration { get; }

    UpsConfiguration UpsConfiguration { get; }

    RegionalConfiguration RegionalConfiguration { get; }

    OnlineIntegrationsConfiguration OnlineIntegrationsConfiguration { get; }

    MainConfiguration MainConfiguration { get; }

    ServerConfiguration ServerConfiguration { get; }


    List<BaseEvent> DomainEvents { get; }

    void RemoveDevice(Device device);

    void AddDevice(Device device);

    void SetBillDispenserConfiguration(IBillDispenserConfiguration billDispenserConfiguration);

    void SetBillAcceptorConfiguration(IBillAcceptorConfiguration billAcceptorConfiguration);

    void SetTITOPrinterConfiguration(ITITOPrinterConfiguration titoPrinterConfiguration);

    void SetCabinetConfiguration(ICabinetConfiguration cabinetConfiguration);
    void SetCoinAcceptorConfiguration(ICoinAcceptorConfiguration coinAcceptorConfiguration);

    void SetUserCardReaderConfiguration(IUserCardReaderConfiguration cardReaderConfiguration);
    void SetParcelLockerConfiguration(IParcelLockerConfiguration parcelLockerConfig);

    string SetNetworkConfiguration(NetworkConfiguration networkConfiguration);

    void SetUpsConfiguration(UpsConfiguration upsConfiguration);

    void SetRegionalConfiguration(RegionalConfiguration regionalConfiguration);

    void SetOnlineIntegrationsConfiguration(OnlineIntegrationsConfiguration onlineIntegrationsConfiguration);

    void SetServerConfiguration(ServerConfiguration serverConfiguration);

    void SetMainConfiguration(MainConfiguration mainConfiguration);

    void AddDeviceError(DeviceFailEvent deviceFailEvent);

    void SetOperatingMode(TerminalOperatingMode operatingMode);

    IBasicHardwareDeviceConfiguration? GetDeviceConfiguration(DeviceType deviceType);

    IBasicHardwareDeviceConfiguration CreateAndValidateDeviceConfiguration(DeviceType deviceType, JsonDocument configurationJson);

    void AddWarningRaised(DeviceWarningRaisedEvent warningRaisedEvent);

    void SetTerminalStatus(string status);

    void PrintMessage(string p1);
    /// <summary>
    /// Update user widgets configuration without notifying.
    /// </summary>
    void UpdateUserWidgetsConfiguration(UserWidgetsConfiguration configuration);

    /// <summary>
    /// Update available user widgets configuration and notify.
    /// </summary>
    void SetUserWidgetsConfiguration(UserWidgetsConfiguration configuration);

    void SetAvailableUserWidgetsConfiguration(AvailableUserWidgetsConfiguration configuration);

    #region Regional Configuration
    void UpdateLocalTimeZone(string? timeZone);
    void UpdateAmountPrecision(int amountPrecision);
    #endregion

    #region Server actions
    List<TerminalServerAction> CompletedServerActions { get; }
    Task AddServerActionAsync(TerminalServerAction serverAction);
    Task RemoveServerActionAsync(Guid uuid);
    #endregion
}