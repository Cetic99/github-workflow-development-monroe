using CashVault.Domain.Aggregates.MessageAggregate.Events;
using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashVault.Domain.Aggregates.MessageAggregate
{
    public class Message : Entity, IAggregateRoot
    {
        private List<BaseEvent> _domainEvents = [];

        [NotMapped]
        public List<BaseEvent> DomainEvents => _domainEvents ?? [];

        public Guid Guid { get; protected set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public string LanguageCode { get; private set; }

        public Message(string languageCode, string key, string value)
        {
            Guid = Guid.NewGuid();
            Key = key;
            Value = value;
            LanguageCode = languageCode;
        }

        public void SetValue(string value)
        {
            if (!string.IsNullOrEmpty(value))
            { 
                Value = value;
                AddDomainEvent(new MessageUpdatedEvent(LanguageCode, Key, Value));
            }
        }

        public void AddDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents ??= [];
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
