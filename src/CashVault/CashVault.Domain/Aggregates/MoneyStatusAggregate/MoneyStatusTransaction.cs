using System;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate
{
    public class MoneyStatusTransaction : Entity
    {
        private Guid _guid;
        private string? _userEmail;
        private string? _username;
        private DispenserBillCountStatus? _oldDispenserBillCountStatus;
        private DispenserBillCountStatus? _newDispenserBillCountStatus;
        private BillTicketAcceptorStackerStatus? _oldbillTicketAcceptorStackerStatus;
        private BillTicketAcceptorStackerStatus? _newBillTicketAcceptorStackerStatus;
        private decimal _amount;
        private Currency _currency;
        private DateTime _timestamp;
        private int transactionStatusId { get; set; }
        public TransactionStatus Status { get; private set; }
        private int moneyStatusTransactionTypeId { get; set; }
        public MoneyStatusTransactionType Type { get; private set; }
        private int deviceTypeId { get; set; }
        public DeviceType DeviceType { get; set; }

        public decimal? OldDeviceBillAmount { get; set; }
        public decimal? NewDeviceBillAmount { get; set; }

        protected MoneyStatusTransaction() { }

        public MoneyStatusTransaction(
            BillTicketAcceptorStackerStatus? oldBillTicketAcceptorStackerStatus,
            BillTicketAcceptorStackerStatus? newBillTicketAcceptorStackerStatus,
            DispenserBillCountStatus? oldDispenserBillCountStatus,
            DispenserBillCountStatus? newDispenserBillCountStatus,
            DeviceType deviceType,
            string? userEmail,
            string? username)
        {
            Guid = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;

            OldBillTicketAcceptorStackerStatus = oldBillTicketAcceptorStackerStatus;
            NewBillTicketAcceptorStackerStatus = newBillTicketAcceptorStackerStatus;

            OldDispenserBillCountStatus = oldDispenserBillCountStatus;
            NewDispenserBillCountStatus = newDispenserBillCountStatus;

            DeviceType = deviceType;

            _userEmail = userEmail;
            _username = username;

            CalculateAmount();
            transactionStatusId = TransactionStatus.Completed.Id;
        }

        public Guid Guid { get => _guid; private set => _guid = value; }
        public BillTicketAcceptorStackerStatus? OldBillTicketAcceptorStackerStatus { get => _oldbillTicketAcceptorStackerStatus; private set => _oldbillTicketAcceptorStackerStatus = value; }
        public BillTicketAcceptorStackerStatus? NewBillTicketAcceptorStackerStatus { get => _newBillTicketAcceptorStackerStatus; private set => _newBillTicketAcceptorStackerStatus = value; }
        public DispenserBillCountStatus? OldDispenserBillCountStatus { get => _oldDispenserBillCountStatus; private set => _oldDispenserBillCountStatus = value; }
        public DispenserBillCountStatus? NewDispenserBillCountStatus { get => _newDispenserBillCountStatus; private set => _newDispenserBillCountStatus = value; }
        public string? UserEmail { get => _userEmail; set => _userEmail = value; }
        public string? Username { get => _username; set => _username = value; }
        public decimal Amount { get => _amount; private set => _amount = value; }
        public Currency Currency { get => _currency; private set => _currency = value; }
        public DateTime Timestamp { get => _timestamp; private set => _timestamp = value; }

        /// <summary>
        /// If new amount is greater than the old amount ==> Credit transaction
        /// If new amount is lower than the old amount ==> Debit transaction
        /// If new amount is equal to the old amount ==> No change
        /// </summary>
        private void CalculateAmount()
        {
            decimal oldDispenserAmount = 0;
            decimal newDispenserAmount = 0;

            decimal oldAcceptorAmount = 0;
            decimal newAcceptorAmount = 0;

            if (OldDispenserBillCountStatus != null && OldDispenserBillCountStatus.Cassettes != null && OldDispenserBillCountStatus.Cassettes.Count > 0)
            {
                foreach (var cassete in OldDispenserBillCountStatus.Cassettes)
                {
                    oldDispenserAmount += cassete.CurrentBillCount * cassete.BillDenomination;

                    Currency = cassete.Currency;
                }
            }

            if (NewDispenserBillCountStatus != null && NewDispenserBillCountStatus.Cassettes != null && NewDispenserBillCountStatus.Cassettes.Count > 0)
            {
                foreach (var cassete in NewDispenserBillCountStatus.Cassettes)
                {
                    newDispenserAmount += cassete.CurrentBillCount * cassete.BillDenomination;

                    Currency = cassete.Currency;
                }
            }

            if (OldBillTicketAcceptorStackerStatus != null)
            {
                oldAcceptorAmount = OldBillTicketAcceptorStackerStatus.BillAmount;
            }

            if (NewBillTicketAcceptorStackerStatus != null)
            {
                newAcceptorAmount = NewBillTicketAcceptorStackerStatus.BillAmount;
            }

            var amount = (newAcceptorAmount + newDispenserAmount) - (oldAcceptorAmount + oldDispenserAmount);
            moneyStatusTransactionTypeId = amount > 0 ? MoneyStatusTransactionType.Refill.Id : MoneyStatusTransactionType.Harvest.Id;

            Amount = Math.Abs(amount);
            Currency = Currency ?? Currency.Default;

            OldDeviceBillAmount = (OldBillTicketAcceptorStackerStatus?.BillAmount ?? 0) + oldDispenserAmount;
            NewDeviceBillAmount = (NewBillTicketAcceptorStackerStatus?.BillAmount ?? 0) + newDispenserAmount;
        }
    }
}
