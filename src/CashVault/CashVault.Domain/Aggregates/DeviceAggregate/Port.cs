using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

public class Port
{
    public PortType PortType { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    public string? SystemPortName { get; private set; }

    public Port(PortType portType, string name, string? description = null, string? systemPortName = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        PortType = portType;
        Name = name;
        Description = description;
        SystemPortName = systemPortName;
    }
}
