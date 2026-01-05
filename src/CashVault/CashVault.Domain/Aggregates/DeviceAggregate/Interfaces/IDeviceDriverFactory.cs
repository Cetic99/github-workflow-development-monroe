using System;
using System.Collections.Generic;
using System.Text.Json;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Interfaces
{
    public interface IDeviceDriverFactory
    {
        MainConfiguration MainConfiguration { get; }
        void SetMainConfiguration(MainConfiguration mainConfig);

        public IBasicHardwareDevice CreateDriver(string fullyQualifiedDriverName, Port port, IBasicHardwareDeviceConfiguration deviceConfigurations, IServiceProvider serviceProvider);

        public List<Port> GetAvailablePorts();

        public List<DeviceModel> GetSupportedDeviceModels();

        public IBasicHardwareDeviceConfiguration? CreateConfiguration(DeviceType deviceType, JsonDocument? config);

        JsonDocument? ConvertConfigurationToJson(IBasicHardwareDeviceConfiguration? deviceConfiguration);
    }
}