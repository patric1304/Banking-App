namespace BankingApp.Services;

using System;
using BankingApp.Models;
using System.Collections.Generic;

public class BankService
{
    private List<Account> accounts = new();

    public void CreateAccount(string accountHolder, AccountType accountType, AccountCurrency accountCurrency, string iban)
    {
        var account = new Account(accountHolder, accountType, accountCurrency, iban);
        accounts.Add(account);
    }

    public bool Deposit(string iban, decimal amount)
    {
        var account = accounts.Find(a => a.Iban == iban);
        if (account == null) return false;
        account.Deposit(amount);
        return true;
    }

    public bool Withdraw(string iban, decimal amount)
    {
        var account = accounts.Find(a => a.Iban == iban);
        if (account == null) return false;
        return account.Withdraw(amount);
    }

    public Account? GetAccount(string iban)
    {
        return accounts.Find(a => a.Iban == iban);
    }

    public List<Account> GetAllAccounts()
    {
        return accounts;
    }

    public bool ChangeCurrency(string iban, AccountCurrency newCurrency, Func<AccountCurrency, AccountCurrency, decimal> rateProvider)
    {
        var account = accounts.Find(a => a.Iban == iban);
        if (account == null) return false;
        return account.ChangeCurrency(newCurrency, rateProvider);
    }
}