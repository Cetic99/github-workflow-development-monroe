using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.MessageAggregate.Events
{
    public class MessageUpdatedEvent : TransactionEvent
    {
        public string? LanguageCode { get; private set; }
        public string? Key { get; private set; }
        public string? Value { get; private set; }

        public MessageUpdatedEvent(string languageCode, string key, string value)
            : base($"Message ({languageCode}-{key}) updated with value of \"{value}\")")
        {
            LanguageCode = languageCode;
            Key = key;
            Value = value;
        }
    }
}
