using System;
using System.Collections.Generic;

namespace CashVault.Domain.Aggregates.DeviceAggregate.Configuration;

public class AvailablePostalService
{
    public string Code { get; set; } = string.Empty;

    public AvailablePostalService() { }

    public AvailablePostalService(string code)
    {
        Code = code;
    }
}

public class AvailablePostalServicesConfiguration
{
    /// <summary>
    /// Represented as list of objects so they could be extended in the future.
    /// </summary>
    public List<AvailablePostalService> AvailablePostalServices { get; set; } = [];

    public AvailablePostalServicesConfiguration() { }

    public void Initialize()
    {
        if (AvailablePostalServices is null || AvailablePostalServices.Count == 0)
        {
            AvailablePostalServices = [new AvailablePostalService("XExpress"),
                new AvailablePostalService("EuroExpress"),
                new AvailablePostalService("A2B"),
                new AvailablePostalService("Monroe")];
        }
    }

    public void UpdateAvailablePostalServices(List<string>? availablePostalServices)
    {
        ArgumentNullException.ThrowIfNull(availablePostalServices, nameof(availablePostalServices));

        // at least one available postal service should be available
        if (availablePostalServices.Count == 0)
            throw new ArgumentException(nameof(availablePostalServices));

        AvailablePostalServices = availablePostalServices.ConvertAll(x => new AvailablePostalService(x));
    }
}
