using System;
using System.Collections.Generic;
using System.Linq;
using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.Aggregates.TransactionAggregate;

public class DispenserBillTransaction : BillTransaction
{
    public List<DispenserBillTransactionItem> Items { get; private set; }

    private DispenserBillTransaction() : base()
    {
        Items = [];
    }

    public DispenserBillTransaction(decimal amountRequested, decimal previousCreditAmount, string description, string? externalReference = null, Currency? currency = null)
        : base(amountRequested, TransactionType.Debit, description, previousCreditAmount, externalReference, currency)
    {
        Items = [];
    }


    public void AddItem(DispenserBillTransactionItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (Items.Any(i => i.BillDenomination == item.BillDenomination))
        {
            throw new InvalidOperationException($"Item with denomination {item.BillDenomination} already exists.");
        }

        if (Items.Any(i => i.CassetteNumber == item.CassetteNumber))
        {
            throw new InvalidOperationException($"Item with cassette number {item.CassetteNumber} already exists.");
        }

        Items.Add(item);
    }

    public void AddItemRange(List<DispenserBillTransactionItem> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            AddItem(item);
        }
    }
}
