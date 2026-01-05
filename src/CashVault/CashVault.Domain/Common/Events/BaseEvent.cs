using System;
using System.Text.Json.Serialization;
using MediatR;

namespace CashVault.Domain.Common.Events
{
    public abstract class BaseEvent : INotification
    {
        [JsonIgnore]
        public int Id { get; set; }
        public Guid Guid { get; private set; }
        public string? EventName { get; protected set; }
        public DateTime Created { get; protected set; }
        public string? Message { get; protected set; }
        [JsonIgnore]
        public bool IsSentToRemote { get; set; }
        public string? Type => this.GetType().Name;
        [JsonIgnore]
        public string Json { get; protected set; }
        public string? CreatedByUser { get; set; }
        public string? CreatedByUserFullName { get; set; }
        public string? CreatedByUserCompany { get; set; }

        protected BaseEvent()
        {
            Created = DateTime.UtcNow;
            Guid = Guid.NewGuid();
        }

        protected BaseEvent(string message)
        {
            Created = DateTime.UtcNow;
            Message = message;
            Guid = Guid.NewGuid();
            EventName = this.GetType().Name;
        }

        public void MarkAsSentToRemote()
        {
            IsSentToRemote = true;
        }

        public void SetJson(string json)
        {
            Json = json;
        }
    }
}
