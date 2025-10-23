namespace BankingApp.Models;

using System;
using System.Collections.Generic;
using System.Linq;

public class Bank
{
    public string Name { get; set; }
    public string Swift { get; set; }
    public List<Account> Accounts { get; set; } = new();

    public Bank(string name, string swift)
    {
        Name = name;
        Swift = swift;
    }

    public void OpenAccount(Account account)
    {
        Accounts.Add(account);
    }

    public bool CloseAccount(string iban)
    {
        var account = Accounts.FirstOrDefault(a => a.Iban == iban);
        if (account == null) return false;
        Accounts.Remove(account);
        return true;
    }

    public Account? GetAccount(string iban)
    {
        return Accounts.FirstOrDefault(a => a.Iban == iban);
    }

    public bool Transfer(string fromIban, string toIban, decimal amount, Func<AccountCurrency, AccountCurrency, decimal> rateProvider)
    {
        var fromAccount = GetAccount(fromIban);
        var toAccount = GetAccount(toIban);

        if (fromAccount == null || toAccount == null) return false;

        if (!fromAccount.Withdraw(amount)) return false;


        if (fromAccount.AccountCurrency != toAccount.AccountCurrency)
        {
            decimal rate = rateProvider(fromAccount.AccountCurrency, toAccount.AccountCurrency);
            amount *= rate;
        }

        toAccount.Deposit(amount);
        return true;
    }

    public bool Deposit(string iban, decimal amount)
    {
        var account = Accounts.Find(a => a.Iban == iban);
        if (account == null) return false;
        account.Deposit(amount);
        return true;
    }

    public bool Withdraw(string iban, decimal amount)
    {
        var account = Accounts.Find(a => a.Iban == iban);
        if (account == null) return false;
        return account.Withdraw(amount);
    }

    public bool ChangeCurrency(string iban, AccountCurrency newCurrency, Func<AccountCurrency, AccountCurrency, decimal> rateProvider)
    {
        var account = Accounts.Find(a => a.Iban == iban);
        if (account == null) return false;
        return account.ChangeCurrency(newCurrency, rateProvider);
    }

    public List<Account> GetAccounts()
    {
        return Accounts;
    }
}