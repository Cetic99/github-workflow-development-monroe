using CashVault.Domain.Common;
using System.Text.Json.Serialization;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

[JsonConverter(typeof(EnumerationJsonConverter<PortType>))]
public class PortType : Enumeration
{
    public static PortType Serial = new PortType(1, nameof(Serial).ToLowerInvariant());
    public static PortType USB = new PortType(2, nameof(USB).ToLowerInvariant());
    public static PortType Ethernet = new PortType(3, nameof(Ethernet).ToLowerInvariant());

    public PortType(int id, string name) : base(id, name) { }
}
