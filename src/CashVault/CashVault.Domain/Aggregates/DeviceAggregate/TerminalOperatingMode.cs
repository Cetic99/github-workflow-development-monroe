using System.Text.Json.Serialization;
using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

[JsonConverter(typeof(EnumerationJsonConverter<TerminalOperatingMode>))]
public class TerminalOperatingMode : Enumeration
{
    public static readonly TerminalOperatingMode UnknownUser = new(0, nameof(UnknownUser).ToLowerInvariant());
    public static readonly TerminalOperatingMode Operator = new(1, nameof(Operator).ToLowerInvariant());

    public TerminalOperatingMode() { }

    public TerminalOperatingMode(int id, string code) : base(id, code) { }
}
