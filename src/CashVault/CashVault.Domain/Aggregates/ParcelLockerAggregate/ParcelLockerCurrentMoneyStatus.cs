using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CashVault.Domain.Aggregates.ParcelLockerAggregate
{
    public class ParcelLockerCurrentMoneyStatus : MoneyStatus
    {
        public decimal BillAmount { get; set; }
        public decimal CoinAmount { get; set; }
        public int BillCount { get; set; }
        public int CoinCount { get; set; }

        [JsonIgnore]
        public decimal TotalAmount => BillAmount + CoinAmount;

        public ParcelLockerCurrentMoneyStatus()
        {
            BillAmount = 0;
            CoinAmount = 0;
            BillCount = 0;
            CoinCount = 0;
        }

        [JsonConstructor]
        public ParcelLockerCurrentMoneyStatus(
            decimal billAmount,
            decimal coinAmount,
            int billCount,
            int coinCount)
        {
            BillAmount = billAmount;
            CoinAmount = coinAmount;
            BillCount = billCount;
            CoinCount = coinCount;
        }

        // TODO: Add events

        public void AddBill(decimal amount, int count = 1)
        {
            BillCount += count;
            BillAmount += amount;
        }

        public void AddCoin(decimal amout, int count = 1)
        {
            CoinCount += count;
            CoinAmount += amout;
        }

        public void Empty()
        {
            BillAmount = 0;
            CoinAmount = 0;
            BillCount = 0;
            CoinCount = 0;
        }

        public override void Initialize()
        {
            if (JsonValue == null) return;

            try
            {
                ParcelLockerCurrentMoneyStatus? obj = JsonSerializer.Deserialize<ParcelLockerCurrentMoneyStatus>(JsonValue);

                if (obj is null)
                    return;

                BillAmount = obj.BillAmount;
                CoinAmount = obj.CoinAmount;
                BillCount = obj.BillCount;
                CoinCount = obj.CoinCount;
            }
            catch { }
        }

        public override void ToJsonString()
        {
            JsonValue = JsonSerializer.Serialize(this);
        }
    }
}
