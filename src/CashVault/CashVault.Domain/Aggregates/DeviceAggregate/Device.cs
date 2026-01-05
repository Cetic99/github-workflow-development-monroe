using System;
using System.Text.Json.Serialization;
using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

public class Device
{
    public DeviceType DeviceType { get; private set; }
    public string? SerialNumber { get; private set; }
    public string? FirmwareVersion { get; private set; }
    public DeviceModel Model { get; private set; }
    [JsonIgnore]
    public IBasicHardwareDevice? DeviceDriver { get; private set; }
    public Port? Port { get; private set; }
    public IBasicHardwareDeviceConfiguration? DeviceConfiguration { get; private set; }

    public Device(DeviceModel model, DeviceType deviceType)
    {
        Model = model;
        DeviceType = deviceType;
        model.SetDeviceType(deviceType);
    }

    public Device(string serialNumber, DeviceModel model)
    {
        SerialNumber = serialNumber;
        Model = model;
    }

    public Device(string serialNumber, string firmwareVersion, DeviceModel model)
    {
        SerialNumber = serialNumber;
        FirmwareVersion = firmwareVersion;
        Model = model;
    }

    public void SetSerialNumber(string serialNumber)
    {
        SerialNumber = serialNumber;
    }

    public void SetFirmwareVersion(string firmwareVersion)
    {
        FirmwareVersion = firmwareVersion;
    }

    public void SetPort(Port port)
    {
        Port = port;
        // TODO: dispatch event for device port changed

    }

    public void SetDeviceDriver(IBasicHardwareDevice deviceDriver)
    {
        DeviceDriver?.Dispose();
        DeviceDriver = deviceDriver;
        DeviceDriver.InitializeAsync();
    }

    public void SetDeviceConfiguration(IBasicHardwareDeviceConfiguration deviceConfiguration)
    {
        ArgumentNullException.ThrowIfNull(deviceConfiguration);
        DeviceConfiguration = deviceConfiguration;
    }

    public override string ToString()
    {
        return $"[model: {Model}, serial number: {SerialNumber}]";
    }
}
