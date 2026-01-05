using System;
using CashVault.Domain.Common;

namespace CashVault.Domain.Aggregates.OperatorAggregate
{
    public class IdentificationCard : Entity
    {
        public Guid Guid { get; protected set; }
        public string SerialNumber { get; private set; }
        public string CardIdentifier { get; private set; }
        public string IssuedBy { get; private set; }
        public DateTime IssuedAt { get; private set; }
        public DateTime ValidFrom { get; private set; }
        public DateTime? ValidTo { get; set; }
        public int OperatorId { get; set; }
        private int identificationCardStatusId { get; set; }

        public IdentificationCardStatus Status { get; private set; }
        public DateTime? LastStatusChange { get; private set; }
        public string Remarks { get; private set; }

        /// <summary>
        /// Creating brand new Identification Card.
        /// </summary>
        /// <param name="uuid">Unique identifier of card.</param>
        /// <param name="serialNumber">Card serial number</param>
        /// <param name="validFrom">Date from which card will be valid.</param>
        /// <param name="validTo">Date until card will be valid</param>
        /// <param name="issuedBy">Name or identifier of entity that issued card.</param>
        /// <param name="remarks">Optionally comments regarding target card.</param>

        private IdentificationCard() { }

        public IdentificationCard(
            Guid uuid,
            DateTime validFrom,
            DateTime? validTo,
            string issuedBy,
            string remarks)
        {
            Guid = uuid;
            IssuedBy = issuedBy;
            IssuedAt = DateTime.UtcNow;
            ValidFrom = validFrom;
            ValidTo = validTo;
            identificationCardStatusId = IdentificationCardStatus.Active.Id;
            Remarks = remarks;
        }

        /// <summary>
        /// Blocks the card.
        /// </summary>
        /// <param name="reason">Reason of card blocking</param>
        /// <exception cref="InvalidOperationException">Already blocked card could not be blocked again.</exception>
        public void Block(string reason)
        {
            if (Status == IdentificationCardStatus.Blocked)
            {
                throw new InvalidOperationException("Card is already blocked.");
            }

            Status = IdentificationCardStatus.Blocked;
            LastStatusChange = DateTime.UtcNow;
            Remarks += $"\n Card was blocked due to: {reason}";
        }

        /// <summary>
        /// Enrolls the card with card identifier, guid and serial number.
        /// </summary>
        /// <param name="cardUID">UID from card</param>
        /// <param name="cardIdentifier">Value from card</param>
        public void Enroll(string cardIdentifier, Guid cardGuid, string serialNumber)
        {
            Guid = cardGuid;
            CardIdentifier = cardIdentifier;
            SerialNumber = serialNumber;
        }

        /// <summary>
        /// Activates or reactivats the card.
        /// </summary>
        /// <param name="comments">Optionally comments regarding target card.</param>
        /// <exception cref="InvalidOperationException">Already active card could not be activated again.</exception>
        public void Activate(string? comments = "")
        {
            if (Status == IdentificationCardStatus.Active)
            {
                throw new InvalidOperationException("Card is already active.");
            }

            Status = IdentificationCardStatus.Active;
            LastStatusChange = DateTime.UtcNow;

            ValidFrom = DateTime.UtcNow;
            ValidTo = null;

            if (!string.IsNullOrEmpty(comments))
            {
                Remarks += $"\n Card was activated due to: {comments}";
            }
        }

        public void Deactivate(string? comments = "")
        {
            if (Status == IdentificationCardStatus.Inactive)
            {
                throw new InvalidOperationException("Card is already inactive.");
            }

            Status = IdentificationCardStatus.Inactive;
            ValidTo = DateTime.UtcNow;
            LastStatusChange = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(comments))
            {
                Remarks += $"\n Card was deactivated due to: {comments}";
            }
        }

        public bool IsValid(DateTime dateTime)
        {
            return Status == IdentificationCardStatus.Active
                && ValidFrom <= dateTime && (!ValidTo.HasValue || ValidTo.Value >= dateTime);
        }

        public void GenerateSerialNumber(int sequenceNumber)
        {
            SerialNumber = "C" + sequenceNumber.ToString().PadLeft(4, '0');
        }
    }
}