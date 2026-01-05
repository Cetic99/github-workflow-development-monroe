using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using CashVault.Domain.Aggregates.OperatorAggregate.Events;
using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;

namespace CashVault.Domain.Aggregates.OperatorAggregate
{
    public class Operator : Entity, IAggregateRoot
    {
        private List<BaseEvent> _domainEvents = [];

        public Guid Guid { get; protected set; }
        public bool IsActive { get; private set; }
        public string Username { get; protected set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? Company { get; private set; }
        [JsonIgnore]// to avoid exposing password hash
        public string Password { get; private set; }
        [JsonIgnore]// to avoid exposing password salt
        public string PasswordSalt { get; private set; }

        public List<Permission> Permissions { get => OperatorPermissions != null ? OperatorPermissions.Select(x => x.Permission).ToList() : []; }
        [JsonIgnore]// to avoid circular reference during serialization, should be refactored
        public List<OperatorPermission> OperatorPermissions { get; private set; } = new List<OperatorPermission>();
        public List<IdentificationCard> IdentificationCards { get; set; } = new List<IdentificationCard>();
        public string? Remarks { get; private set; }

        public List<BaseEvent> DomainEvents => _domainEvents ?? [];

        private Operator() { }

        public Operator(Guid guid, string username, string firstName, string lastName, string email, string phoneNumber, string password, string passwordSalt, List<Permission> permissions, string company, string? remarks = "")
        {
            var operatorPermissions = new List<OperatorPermission>();

            foreach (var permission in permissions)
                operatorPermissions.Add(new OperatorPermission(this, permission));

            Guid = guid;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            OperatorPermissions = operatorPermissions;
            Remarks = remarks;
            PasswordSalt = passwordSalt;
            IsActive = true;
            Company = company;

            AddDomainEvent(new OperatorCreatedEvent(this));
        }

        public Operator(Guid guid, string username, string firstName, string lastName, string email, string phoneNumber, string password, string passwordSalt, string? remarks = "")
        {
            Guid = guid;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Remarks = remarks;
            PasswordSalt = passwordSalt;
            IsActive = true;

            AddDomainEvent(new OperatorCreatedEvent(this));
        }

        public void Activate(string? comments = "")
        {
            if (IsActive)
            {
                throw new InvalidOperationException("Operator is already active.");
            }

            IsActive = true;

            if (!string.IsNullOrEmpty(comments))
            {
                Remarks += $"\n Operator was activated due to: {comments}";
            }

            AddDomainEvent(new OperatorActivatedEvent(this));
        }

        public void Deactivate(string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                throw new ArgumentNullException("Reason can not be empty.", nameof(reason));
            }

            if (!IsActive)
            {
                throw new InvalidOperationException("Operator is already deactivated.");
            }

            IsActive = false;
            Remarks += $"\n Operator was deactivated due to: {reason}";

            AddDomainEvent(new OperatorDeactivatedEvent(this));
        }

        public void UpdatePersonalData(string firstName, string lastName, string email, string phoneNumber, string remarks, string company)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Remarks = remarks;
            Company = company;
        }

        public void AddNewCard(IdentificationCard card)
        {
            ArgumentNullException.ThrowIfNull(card);

            //VerifyCard(card);

            if (IdentificationCards.Any(x => x.Guid.Equals(card.Guid)))
                throw new ArgumentException("Identification card is already added");

            IdentificationCards.Add(card);

            AddDomainEvent(new OperatorCardAddedEvent(this, card));
        }

        public void CloseCard(Guid guid, string comments = "")
        {
            var card = IdentificationCards.Find(c => c.Guid == guid);

            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            card.Deactivate(comments);
        }

        public void ActivateCard(Guid? guid, string? comments = "")
        {
            if (!guid.HasValue || Guid.Empty.Equals(guid.Value))
            {
                throw new ArgumentException("Guid cannot be empty or null.", nameof(guid));
            }

            var card = IdentificationCards.Find(c => c.Guid == guid)
                ?? throw new InvalidOperationException("Card not found.");

            card.Activate(comments);
        }

        public void BlockCard(Guid guid, string reason)
        {
            if (guid == Guid.Empty)
            {
                throw new ArgumentException("Guid cannot be empty or null.", nameof(guid));
            }
            if (guid == null)
            {
                throw new ArgumentNullException(nameof(guid));
            }
            if (string.IsNullOrEmpty(reason))
            {
                throw new ArgumentNullException("Reason can not be empty.");
            }

            var card = IdentificationCards.Find(c => c.Guid == guid);

            if (card == null)
            {
                throw new InvalidOperationException("Card not found.");
            }

            card.Block(reason);
        }

        public void UpdatePermissions(List<Permission> newPermissions)
        {
            if (newPermissions == null)
                throw new ArgumentNullException(nameof(newPermissions));

            var op = new List<OperatorPermission>();

            foreach (var permission in newPermissions)
                op.Add(new OperatorPermission(this, permission));

            OperatorPermissions = op;
            AddDomainEvent(new OperatorPermissionsUpdatedEvent(this, newPermissions));
        }

        public void SetPassword(string password, string passwordSalt)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Password cannot be empty.", nameof(password));
            }

            if (string.IsNullOrEmpty(passwordSalt))
            {
                throw new ArgumentNullException("Password cannot be empty.", nameof(passwordSalt));
            }

            Password = password;
            PasswordSalt = passwordSalt;
            AddDomainEvent(new OperatorPasswordUpdatedEvent(this));
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }


        public bool VerifyCard(IdentificationCard? card)
        {
            ArgumentNullException.ThrowIfNull(card, "IdentificationCard is null");

            if (card.Status == IdentificationCardStatus.Blocked) throw new InvalidOperationException("Card is blocked and cannot be used for authentication.");

            if (!card.IsValid(DateTime.UtcNow)) throw new InvalidOperationException("Card is not valid at this time.");

            return true;
        }

        public void AuthenticateWithIdentificationCard(Guid cardUuid)
        {
            var card = IdentificationCards.Find(c => c.Guid == cardUuid);
            var cardValid = VerifyCard(card);

            if (cardValid && IsActive) AddDomainEvent(new OperatorAuthenticatedEvent(this));

            if (!IsActive)
            {
                AddDomainEvent(new OperatorAuthenticationFailedEvent(cardUuid));
            }
        }

        public void AuthenticateWithCredentials(bool credentialsVerified)
        {
            if (credentialsVerified && IsActive) AddDomainEvent(new OperatorAuthenticatedEvent(this));

            if (!IsActive)
            {
                AddDomainEvent(new OperatorAuthenticationFailedEvent(username: Username));
                throw new InvalidOperationException($"Operator ({GetFullName()}) is not active");
            }

            if (!credentialsVerified)
            {
                AddDomainEvent(new OperatorAuthenticationFailedEvent(username: Username));
                throw new InvalidOperationException($"Credentials for operator ({GetFullName()}) are not valid");
            }
        }

        public void HarvestShiftMoney()
        {
            AddDomainEvent(new OperatorShiftMoneyHarvestedEvent(this));
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