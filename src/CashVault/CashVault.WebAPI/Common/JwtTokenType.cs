using CashVault.Domain.Common;

namespace CashVault.WebAPI.Common;

public class JwtTokenType : Enumeration
{
    public static readonly JwtTokenType Access = new(1, nameof(Access).ToLowerInvariant());
    public static readonly JwtTokenType Refresh = new(2, nameof(Refresh).ToLowerInvariant());

    public JwtTokenType(int id, string code) : base(id, code)
    {
    }
}
