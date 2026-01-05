using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events
{
    public class CassetteAddedEvent : TransactionEvent
    {
        public CassetteAddedEvent(DispenserCassetteBillCountStatus cassette)
             : base(FormatMessage(cassette))
        {
            Cassette = cassette;
        }

        private static string FormatMessage(DispenserCassetteBillCountStatus cassette) =>
            $"Cassette added [cassette number: {cassette.CassetteNumber}, " +
            $"bill denomination: {cassette.BillDenomination}, " +
            $"bill count: {cassette.CurrentBillCount}, " +
            $"currency: {cassette.Currency.Code}]";

        public DispenserCassetteBillCountStatus Cassette { get; private set; }
    }
}
