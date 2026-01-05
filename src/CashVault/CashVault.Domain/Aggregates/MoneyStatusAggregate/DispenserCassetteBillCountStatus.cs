using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate
{
    public class DispenserCassetteBillCountStatus
    {
        public int CassetteNumber { get; set; }
        public Currency Currency { get; set; }
        public int BillDenomination { get; set; }
        public int CurrentBillCount { get; set; }

        public DispenserCassetteBillCountStatus(int cassetteNumber, Currency currency, int billDenomination, int currentBillCount)
        {
            CassetteNumber = cassetteNumber;
            Currency = currency;
            BillDenomination = billDenomination;
            CurrentBillCount = currentBillCount;
        }

        public void UpdateBillCount(int newBillCount)
        {
            CurrentBillCount = newBillCount;
        }

        public void UpdateBillDenomination(int newBillDenomination)
        {
            BillDenomination = newBillDenomination;
        }

        public void UpdateCurrency(Currency newCurrency)
        {
            Currency = newCurrency;
        }

        public void IncreaseBillCount(int count)
        {
            CurrentBillCount += count;
        }

        public void DecreaseBillCount(int count)
        {
            if (CurrentBillCount - count < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(count), "Cannot decrease bill count below zero.");
            }

            CurrentBillCount -= count;
        }
    }
}
