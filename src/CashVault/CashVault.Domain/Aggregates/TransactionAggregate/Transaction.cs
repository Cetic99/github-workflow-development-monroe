using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CashVault.Domain.Aggregates.TransactionAggregate.Events;
using CashVault.Domain.Common;
using CashVault.Domain.Common.Events;
using CashVault.Domain.ValueObjects;

namespace CashVault.Domain.TransactionAggregate;

public abstract class Transaction : Entity, IAggregateRoot
{
    private List<BaseEvent> _domainEvents = [];

    public Guid Guid { get; protected set; }
    public DateTime ProcessingStarted { get; private set; }
    public DateTime? ProcessingEnded { get; private set; }
    public decimal AmountRequested { get; private set; }
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }
    private int transactionStatusId { get; set; }
    public TransactionStatus Status { get; private set; }
    private int transactionTypeId { get; set; }
    public TransactionType Type { get; private set; }
    public string? Description { get; private set; }
    public string? ExternalReference { get; private set; }
    public decimal PreviousCreditAmount { get; private set; } = 0;
    public decimal NewCreditAmount { get; private set; } = 0;

    [NotMapped]
    public List<BaseEvent> DomainEvents => _domainEvents ?? [];

    protected Transaction()
    {
    }

    public Transaction(decimal amountRequested, TransactionType type, string description, decimal previousCreditAmount, string? externalReference = null, Currency? currency = null)
    {
        Guid = Guid.NewGuid();
        transactionStatusId = TransactionStatus.Pending.Id;
        ProcessingStarted = DateTime.UtcNow;
        AmountRequested = amountRequested;
        Amount = AmountRequested;
        Currency = currency ?? Currency.Default;
        transactionTypeId = type.Id;
        Description = description;
        ExternalReference = externalReference;
        PreviousCreditAmount = previousCreditAmount;
    }

    public void CompleteTransaction(decimal newCreditAmount, string? remarks = "")
    {
        Status = TransactionStatus.Completed;
        ProcessingEnded = DateTime.UtcNow;
        NewCreditAmount = newCreditAmount;
        if (!string.IsNullOrEmpty(remarks))
        {
            Description += $"\n{remarks}";
        }
        AddDomainEvent(new TransactionCompletedEvent(this));
    }

    public void CompleteTransaction(decimal amount, decimal newCreditAmount, string? remarks = "")
    {
        if (amount == 0 && AmountRequested != 0)
        {
            FailTransaction("Amount is zero", newCreditAmount);
            return;
        }

        if (AmountRequested == amount)
        {
            Status = TransactionStatus.Completed;
        }
        else
        {
            Status = TransactionStatus.PartiallyCompleted;
        }
        Amount = amount;
        NewCreditAmount = newCreditAmount;
        ProcessingEnded = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(remarks))
        {
            Description += $"\n{remarks}";
        }
        AddDomainEvent(new TransactionCompletedEvent(this));
    }

    public void FailTransaction(string reason, decimal newCreditAmount)
    {
        Status = TransactionStatus.Failed;
        ProcessingEnded = DateTime.UtcNow;
        NewCreditAmount = newCreditAmount;
        if (!string.IsNullOrEmpty(reason))
        {
            Description += $"\n{reason}";
        }
        AddDomainEvent(new TransactionFailedEvent(this, reason));
    }

    public override string ToString()
    {
        return $"Transaction Id: {Id}, Guid: {Guid}, Amount: {Amount}, Status: {Status}, Description: {Description}, Reference: {ExternalReference}";
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