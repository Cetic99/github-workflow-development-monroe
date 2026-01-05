namespace CashVault.Domain.Common.Events
{
    public class TransactionEvent : BaseEvent
    {
        public TransactionEvent(string message)
            : base(message)
        {
            Message = message;
        }

        protected TransactionEvent() : base() { }
    }
}
