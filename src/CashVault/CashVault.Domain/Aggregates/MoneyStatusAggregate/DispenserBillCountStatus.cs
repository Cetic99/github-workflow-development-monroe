using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using CashVault.Domain.Aggregates.DeviceAggregate.Events;
using CashVault.Domain.Aggregates.MoneyStatusAggregate.Events;
using CashVault.Domain.Aggregates.MoneyStatusAggregate.Exceptions;
using CashVault.Domain.Aggregates.TransactionAggregate;

namespace CashVault.Domain.Aggregates.MoneyStatusAggregate;

public class DispenserBillCountStatus : MoneyStatus, ICloneable
{
    public List<DispenserCassetteBillCountStatus> Cassettes { get; set; }
    public DispenserRejectBinStatus RejectBin { get; set; }

    [JsonConstructor]
    public DispenserBillCountStatus(List<DispenserCassetteBillCountStatus>? cassettes = null, DispenserRejectBinStatus? rejectBin = null)
    {
        Cassettes = cassettes ?? [];
        RejectBin = rejectBin ?? new DispenserRejectBinStatus();
    }

    private DispenserBillCountStatus() { }

    public List<DispenserBillTransactionItem> CalculateDispenseSpecifications(decimal amountRequested, List<DenominationCount>? billSpecification = null)
    {
        if (amountRequested <= 0)
            throw new ArgumentOutOfRangeException(nameof(amountRequested), "Amount requested must be greater than zero.");

        if (IsEmpty())
            throw new InsufficientDispenserBillsException("Dispenser is empty.");

        var result = new List<DispenserBillTransactionItem>();
        var remainingAmount = amountRequested;

        if (billSpecification == null)
        {
            // Default logic if no billSpecification is provided
            foreach (var cassette in Cassettes.OrderByDescending(c => c.BillDenomination))
            {
                var billCount = (int)Math.Floor(remainingAmount / cassette.BillDenomination);
                if (billCount > cassette.CurrentBillCount)
                {
                    billCount = cassette.CurrentBillCount;
                }

                if (billCount > 0)
                {
                    result.Add(new DispenserBillTransactionItem(cassette.CassetteNumber, cassette.BillDenomination, billCount));
                    remainingAmount -= billCount * cassette.BillDenomination;
                }

                if (remainingAmount == 0)
                {
                    break;
                }
            }

            if (remainingAmount > 0)
            {
                throw new InsufficientDispenserBillsException($"Insufficient bills in dispenser to dispense {amountRequested}.");
            }

            return result;
        }
        else
        {
            // Validate and use user-specified denominations
            foreach (var spec in billSpecification)
            {
                var cassette = Cassettes.FirstOrDefault(c => c.BillDenomination == spec.Denomination);
                if (cassette == null)
                {
                    throw new InvalidOperationException($"Cassette with denomination {spec.Denomination} does not exist.");
                }

                if (spec.Count > cassette.CurrentBillCount)
                {
                    throw new InvalidOperationException($"Not enough bills in cassette for denomination {spec.Denomination}.");
                }

                if (spec.Count > 0)
                {
                    result.Add(new DispenserBillTransactionItem(cassette.CassetteNumber, cassette.BillDenomination, spec.Count));
                    remainingAmount -= spec.Count * cassette.BillDenomination;
                }
            }

            if (remainingAmount > 0)
            {
                throw new InsufficientDispenserBillsException($"Insufficient bills in dispenser to dispense {amountRequested}.");
            }

            return result;
        }
    }

    public List<DenominationDispenseOption> GetDenominationDispenseOptions(decimal amountHint)
    {
        var options = new List<DenominationDispenseOption>();

        if (amountHint <= 0)
        {
            return options;
        }

        foreach (var cassette in Cassettes.OrderByDescending(c => c.BillDenomination))
        {
            var billCount = (int)Math.Floor(amountHint / cassette.BillDenomination);
            if (billCount > cassette.CurrentBillCount)
            {
                billCount = cassette.CurrentBillCount;
            }

            if (billCount > 0)
            {
                var maxAmount = billCount * cassette.BillDenomination;
                options.Add(new DenominationDispenseOption(cassette.BillDenomination, billCount, maxAmount));
            }
        }

        return options;
    }

    public List<DenominationCount> GetPrefilledCombinationsForAmount(decimal amountRequested)
    {
        var result = new List<DenominationCount>();
        var remainingAmount = amountRequested;

        if (amountRequested <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amountRequested), "Amount requested must be greater than zero.");
        }

        if (IsEmpty())
        {
            throw new InsufficientDispenserBillsException("Dispenser is empty.");
        }

        foreach (var cassette in Cassettes.OrderByDescending(c => c.BillDenomination))
        {
            var billCount = (int)Math.Floor(remainingAmount / cassette.BillDenomination);
            if (billCount > cassette.CurrentBillCount)
            {
                billCount = cassette.CurrentBillCount;
            }

            result.Add(new DenominationCount(cassette.BillDenomination, billCount));

            if (billCount > 0)
            {
                remainingAmount -= billCount * cassette.BillDenomination;
            }

            if (remainingAmount == 0)
            {
                break;
            }
        }

        foreach (var cassette in Cassettes.OrderByDescending(c => c.BillDenomination))
        {
            if (!result.Any(dc => dc.Denomination == cassette.BillDenomination))
            {
                result.Add(new DenominationCount(cassette.BillDenomination, 0));
            }
        }

        return result;
    }

    public void UpdateCassetteBillCountFromTransaction(DispenserBillTransaction transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        foreach (var item in transaction.Items)
        {
            var cassette = Cassettes.FirstOrDefault(c => c.CassetteNumber == item.CassetteNumber);
            if (cassette == null)
            {
                throw new InvalidOperationException($"Cassette with number {item.CassetteNumber} does not exist.");
            }

            var oldStatus = this.Clone() as DispenserBillCountStatus;
            var billCount = item.BillCountDispensed + item.BillCountRejected;
            cassette.DecreaseBillCount(billCount);
            var newStatus = this.Clone() as DispenserBillCountStatus;
            AddDomainEvent(new CassetteBillCountDecreasedEvent(cassette, billCount, oldStatus, newStatus));

            //reject bin handling
            if (item.BillCountRejected > 0)
            {
                var statusBeforeRejecting = this.Clone() as DispenserBillCountStatus;
                RejectBin.AddSource(new RejectBinSource(cassette.CassetteNumber, cassette.BillDenomination, item.BillCountRejected));
                var statusAfterRejecting = this.Clone() as DispenserBillCountStatus;
                AddDomainEvent(new CassetteBillsRejectedEvent(cassette, item.BillCountRejected, statusBeforeRejecting, statusAfterRejecting));
            }
        }
    }

    public DispenserCassetteBillCountStatus EmptyCassette(int cassetteNumber)
    {
        var cassette = Cassettes.FirstOrDefault(c => c.CassetteNumber == cassetteNumber);
        if (cassette == null)
        {
            throw new InvalidOperationException($"Cassette with number {cassetteNumber} does not exist.");
        }

        var oldStatus = this.Clone() as DispenserBillCountStatus;
        var oldBillCount = cassette.CurrentBillCount;
        cassette.CurrentBillCount = 0;
        var newStatus = this.Clone() as DispenserBillCountStatus;

        AddDomainEvent(new CassetteEmptiedEvent(cassette, oldBillCount, oldStatus, newStatus));
        return cassette;
    }

    public void Empty()
    {
        foreach (var cassette in Cassettes)
        {
            cassette.CurrentBillCount = 0;
        }
    }

    public bool IsEmpty()
    {
        return Cassettes.Sum(c => c.CurrentBillCount) == 0;
    }

    public void UpdateBillCassetteFromDispenserConfig(List<BillCasseteConfiguration> cassetteConfig)
    {
        if (cassetteConfig == null)
            throw new ArgumentNullException(nameof(cassetteConfig));

        foreach (var singleCassetteConfig in cassetteConfig)
        {
            var cassetteStatus = Cassettes.FirstOrDefault(c => c.CassetteNumber == singleCassetteConfig.CassetteNumber);
            if (cassetteStatus == null)
            {
                // Add new cassette
                AddCassette(new DispenserCassetteBillCountStatus(
                    singleCassetteConfig.CassetteNumber,
                    singleCassetteConfig.BillDenomination.Currency,
                    singleCassetteConfig.BillDenomination.Value,
                    0
                ));
            }
            else
            {
                // Update existing cassette
                cassetteStatus.BillDenomination = singleCassetteConfig.BillDenomination.Value;
                cassetteStatus.Currency = singleCassetteConfig.BillDenomination.Currency;
            }
        }

        // // Remove any cassettes that are not in the new configuration

        var newCassetteNumbers = cassetteConfig.Select(c => c.CassetteNumber).ToList();
        var numbersToRemove = Cassettes.Where(c => !newCassetteNumbers.Contains(c.CassetteNumber)).Select(c => c.CassetteNumber).ToList();
        foreach (var cassetteNumber in numbersToRemove)
        {
            RemoveCassette(cassetteNumber);
        }

        AddDomainEvent(new CassettesUpdatedFromDispnserConfigEvent());
    }

    public void AddCassette(DispenserCassetteBillCountStatus cassette)
    {
        if (cassette == null)
            throw new ArgumentNullException(nameof(cassette));

        if (Cassettes.Any(c => c.CassetteNumber == cassette.CassetteNumber))
        {
            throw new InvalidOperationException($"Cassette with number {cassette.CassetteNumber} already exists.");
        }

        Cassettes.Add(cassette);
        AddDomainEvent(new CassetteAddedEvent(cassette));
    }

    public DispenserCassetteBillCountStatus RefillCassette(int cassetteNumber, int billCount)
    {
        if (billCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(billCount), "Bill count must be greater than zero.");

        var cassette = Cassettes.FirstOrDefault(c => c.CassetteNumber == cassetteNumber);
        if (cassette == null)
        {
            throw new InvalidOperationException($"Cassette with number {cassetteNumber} does not exist.");
        }

        var oldStatus = this.Clone() as DispenserBillCountStatus;

        cassette.IncreaseBillCount(billCount);
        var newStatus = this.Clone() as DispenserBillCountStatus;
        AddDomainEvent(new CassetteRefilledEvent(cassette, billCount, oldStatus, newStatus));
        return cassette;
    }

    public void UpdateCassette(DispenserCassetteBillCountStatus cassette)
    {
        if (cassette == null)
            throw new ArgumentNullException(nameof(cassette));

        var existingCassette = Cassettes.FirstOrDefault(c => c.CassetteNumber == cassette.CassetteNumber);
        if (existingCassette == null)
        {
            throw new InvalidOperationException($"Cassette with number {cassette.CassetteNumber} does not exist.");
        }

        existingCassette = cassette;
    }

    public DispenserCassetteBillCountStatus RemoveCassette(int cassetteNumber)
    {
        var cassette = Cassettes.FirstOrDefault(c => c.CassetteNumber == cassetteNumber);
        if (cassette == null)
        {
            throw new InvalidOperationException($"Cassette with number {cassetteNumber} does not exist.");
        }

        Cassettes.Remove(cassette);
        AddDomainEvent(new CassetteRemovedEvent(cassette));
        return cassette;
    }

    public void EmptyRejectBin()
    {
        var billCount = RejectBin.BillCount;
        var oldStatus = this.Clone() as DispenserBillCountStatus;
        RejectBin.Clear();
        var newStatus = this.Clone() as DispenserBillCountStatus;
        AddDomainEvent(new BillDispenserRejectBinEmptiedEvent(billCount, oldStatus, newStatus));
    }

    public override void ToJsonString()
    {
        JsonValue = System.Text.Json.JsonSerializer.Serialize(this);
    }

    public override void Initialize()
    {
        if (JsonValue == null)
        {
            return;
        }
        DispenserBillCountStatus? obj = JsonSerializer.Deserialize<DispenserBillCountStatus>(JsonValue);
        Cassettes = obj?.Cassettes ?? [];
        RejectBin = obj?.RejectBin ?? new DispenserRejectBinStatus();
    }

    public object Clone()
    {
        var json = JsonSerializer.Serialize(this);
        return JsonSerializer.Deserialize<DispenserBillCountStatus>(json);
    }
}

public class DenominationDispenseOption
{
    public int BillDenomination { get; set; }
    public int MaxBillCount { get; set; }
    public decimal MaxAmount { get; set; }

    public DenominationDispenseOption(int billDenomination, int maxBillCount, decimal maxAmount)
    {
        BillDenomination = billDenomination;
        MaxBillCount = maxBillCount;
        MaxAmount = maxAmount;
    }
}

public class DenominationCount
{
    public int Denomination { get; set; }
    public int Count { get; set; }

    public DenominationCount(int denomination, int count)
    {
        Denomination = denomination;
        Count = count;
    }
}
