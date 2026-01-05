using System.Text.Json.Serialization;
using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.DeviceAggregate;

[JsonConverter(typeof(EnumerationJsonConverter<TerminalType>))]
public class TerminalType : Enumeration
{
    public static TerminalType GamingATM = new(1, nameof(GamingATM).ToLowerInvariant());
    public static TerminalType ParcelLocker = new(2, nameof(ParcelLocker).ToLowerInvariant());
    public static TerminalType Entertainment = new(3, nameof(Entertainment).ToLowerInvariant());
    public static TerminalType BankingAtm = new(4, nameof(BankingAtm).ToLowerInvariant());

    public TerminalType() { }

    public TerminalType(int id, string code)
        : base(id, code) { }
}
