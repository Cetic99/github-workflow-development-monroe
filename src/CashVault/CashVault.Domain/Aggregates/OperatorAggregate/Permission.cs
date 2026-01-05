using CashVault.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashVault.Domain.Aggregates.OperatorAggregate;

public class Permission : Enumeration
{
    public static Permission BillAcceptor { get; } = new Permission(1, nameof(BillAcceptor).ToLowerInvariant());
    public static Permission BillDispenser { get; } = new Permission(2, nameof(BillDispenser).ToLowerInvariant());
    public static Permission CoinDispenser { get; } = new Permission(3, nameof(CoinDispenser).ToLowerInvariant());
    public static Permission CardReader { get; } = new Permission(3, nameof(CardReader).ToLowerInvariant());
    public static Permission MoneyService { get; } = new Permission(3, nameof(MoneyService).ToLowerInvariant());
    public static Permission Reports { get; } = new Permission(3, nameof(Reports).ToLowerInvariant());
    public static Permission Administration { get; } = new Permission(3, nameof(Administration).ToLowerInvariant());
    public static Permission Configuration { get; } = new Permission(3, nameof(Configuration).ToLowerInvariant());
    public static Permission Shutdown { get; } = new Permission(3, nameof(Shutdown).ToLowerInvariant());
    public static Permission Maintenance { get; } = new Permission(3, nameof(Maintenance).ToLowerInvariant());

    public Permission(int id, string code) : base(id, code) { }
}
