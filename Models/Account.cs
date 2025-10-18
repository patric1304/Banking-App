namespace BankingApp.Models;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public enum AccountType { Person, Company }
public enum AccountCurrency { RON, EUR, USD, GBP }

public class Transaction
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class Account
{
    public string AccountHolder { get; set; }
    public AccountType AccountType { get; set; }
    public AccountCurrency AccountCurrency { get; set; }
    public string Iban { get; set; }
    public decimal Amount { get; set; }

    public List<Transaction> Transactions { get; set; } = new();

    public decimal AccountFeeCurrencyChange { get; set; } = 0.01m;

    [JsonConstructor]
    public Account(string accountHolder, AccountType accountType, AccountCurrency accountCurrency, string iban)
    {
        AccountHolder = accountHolder;
        AccountType = accountType;
        AccountCurrency = accountCurrency;
        Iban = iban;
        Amount = 0m;
    }

    public void Deposit(decimal value)
    {
        Amount += value;
        Transactions.Add(new Transaction { Amount = value, Description = "Deposit" });
    }

    public bool Withdraw(decimal value)
    {
        if (Amount < value) return false;
        Amount -= value;
        Transactions.Add(new Transaction { Amount = -value, Description = "Withdraw" });
        return true;
    }

    public bool ChangeCurrency(AccountCurrency newCurrency, Func<AccountCurrency, AccountCurrency, decimal> rateProvider)
    {
        if (newCurrency == AccountCurrency) return true;
        var rate = rateProvider(AccountCurrency, newCurrency);
        var fee = Amount * AccountFeeCurrencyChange;
        var remaining = Amount - fee;
        if (remaining <= 0) return false;
        var newAmount = remaining * rate;
        Amount = Math.Round(newAmount, 2);
        AccountCurrency = newCurrency;
        Transactions.Add(new Transaction { Amount = Amount, Description = $"Currency change to {newCurrency} (fee {fee})" });
        return true;
    }

    public override string ToString()
    {
        return $"IBAN: {Iban} | Holder: {AccountHolder} | Type: {AccountType} | Currency: {AccountCurrency} | Amount: {Amount}";
    }
}