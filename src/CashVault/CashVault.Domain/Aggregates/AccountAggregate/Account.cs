using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace CashVault.Domain.Aggregates.AccountAggregate;

public class Account
{
    public Guid Guid { get; protected set; }
    public string Username { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public decimal Balance { get; private set; }
    public Currency Currency { get; private set; } = Currency.Default;

    /// <summary>
    /// Constructor used when user is totally unknown
    /// </summary>
    public Account()
    {
        Guid = Guid.Empty;
        Username = "Unknown";
        FirstName = "Unknown";
        LastName = "Unknown";
        Balance = 0;
    }

    /// <summary>
    /// Constructor used when new user is created with appropriate role
    /// </summary>
    /// <param name="username"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="email"></param>
    /// <param name="role"></param>
    public Account(string username, string firstName, string lastName, string email)
    {
        Guid = Guid.NewGuid();
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    /// <summary>
    /// Constructor used when user is already known
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="email"></param>
    /// <param name="balance"></param>
    /// <param name="currency"></param>
    /// <param name="role"></param>
    public Account(Guid id, string username, string firstName, string lastName, string email, decimal balance = 0, Currency? currency = null)
    {
        Guid = id;
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Balance = balance;
        Currency = currency != null ? currency : Currency.Default;
    }

    /// <summary>
    /// Method for depositing money to account
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseBalance(decimal amount)
    {
        Balance += amount;
    }

    /// <summary>
    /// Method for withdrawing money from account
    /// </summary>
    /// <param name="amount"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void DecreaseBalance(decimal amount)
    {
        if (amount > Balance)
            throw new InvalidOperationException("Insufficient funds.");

        Balance -= amount;
    }

}
