using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate.Events
{
    public class CassetteRemovedEvent : TransactionEvent
    {
        public CassetteRemovedEvent(DispenserCassetteBillCountStatus cassette)
            : base($"Cassette ${cassette.CassetteNumber} removed")
        {
            Cassette = cassette;
        }

        public DispenserCassetteBillCountStatus Cassette { get; private set; }
    }
}
