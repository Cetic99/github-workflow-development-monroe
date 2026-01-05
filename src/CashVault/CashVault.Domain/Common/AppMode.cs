namespace CashVault.Domain.Common;

public class AppMode : Enumeration
{
    public static readonly AppMode Operator = new(1, nameof(Operator));
    public static readonly AppMode UnknownUser = new(2, nameof(UnknownUser));
    public static readonly AppMode User = new(3, nameof(User));
    public static readonly AppMode Safe = new(4, nameof(Safe));

    public AppMode(int id, string code) : base(id, code)
    {
    }
}
