using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate
{
    public abstract class MoneyStatus : Entity, IAggregateRoot
    {
        private List<BaseEvent> _domainEvents = [];

        public int Id { get; private set; }

        public string Key { get; private set; }

        [JsonIgnore]
        public string JsonValue
        {
            get;
            protected set;
        }

        [JsonIgnore]
        public List<BaseEvent> DomainEvents => _domainEvents ?? [];

        #region Constructors
        protected MoneyStatus() { }
        #endregion
        public void AddDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents ??= [];
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// Serializes the current instance of <see cref="MoneyStatus"/> to a JSON string.
        /// </summary>
        /// <returns>A JSON string representation of the current instance.</returns>
        public abstract void ToJsonString();

        /// <summary>
        /// Deserializes the provided JSON string to update the current instance of <see cref="MoneyStatus"/>.
        /// Used for load properties from the database!
        /// </summary>
        public abstract void Initialize();
    }
}
