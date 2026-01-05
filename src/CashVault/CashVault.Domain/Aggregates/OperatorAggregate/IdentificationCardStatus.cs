using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.OperatorAggregate;

public class IdentificationCardStatus : Enumeration
{
    public static IdentificationCardStatus Active { get; } = new IdentificationCardStatus(1, nameof(Active).ToLowerInvariant());
    public static IdentificationCardStatus Blocked { get; } = new IdentificationCardStatus(2, nameof(Blocked).ToLowerInvariant());
    public static IdentificationCardStatus Inactive { get; } = new IdentificationCardStatus(3, nameof(Inactive).ToLowerInvariant());

    public IdentificationCardStatus(int id, string code) : base(id, code) { }
}
