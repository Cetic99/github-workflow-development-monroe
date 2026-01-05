using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.MessageAggregate
{
    public class Language : Enumeration
    {

        public static readonly Language English = new(2, "us");
        public static readonly Language Serbian = new(3, "bs");

        public static Language Default = new(99, Serbian.Code);
        public Language(int id, string name) : base(id, name) { }
    }
}
