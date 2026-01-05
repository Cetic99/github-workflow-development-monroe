using System;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

public class DeviceModel
{
    public DeviceType DeviceType { get; private set; }
    public string Name => Manufacturer + " " + Model;
    public string FullyQualifiedDriverName { get; private set; }
    public string Manufacturer { get; private set; }
    public string Model { get; private set; }
    public string Description { get; private set; }

    public DeviceModel(DeviceType deviceType, string manufacturer, string model, string fullyQualifiedDriverName, string description)
    {
        if (string.IsNullOrWhiteSpace(manufacturer))
        {
            throw new ArgumentNullException(nameof(manufacturer));
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (string.IsNullOrWhiteSpace(fullyQualifiedDriverName))
        {
            throw new ArgumentNullException(nameof(fullyQualifiedDriverName));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentNullException(nameof(description));
        }

        DeviceType = deviceType;
        Manufacturer = manufacturer;
        Model = model;
        FullyQualifiedDriverName = fullyQualifiedDriverName;
        Description = description;
    }

    public void SetDeviceType(DeviceType deviceType) => DeviceType = deviceType;
}
