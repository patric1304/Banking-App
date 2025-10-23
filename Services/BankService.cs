namespace BankingApp.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BankingApp.Models;

public class BankService
{
    private List<Bank> _banks = new();
    private readonly string _dataFile = "banks.json";

    public BankService()
    {
        LoadData();
    }

    public List<Bank> GetAllBanks()
    {
        return _banks;
    }

    public Bank? GetBank(string name)
    {
        return _banks.FirstOrDefault(b => b.Name == name);
    }

    public void CreateBank(string name, string swift)
    {
        var bank = new Bank(name, swift);
        _banks.Add(bank);
        SaveData();
    }

    public void CreateAccount(string bankName, string accountHolder, AccountType accountType, AccountCurrency currency)
    {
        var bank = GetOrCreateBank(bankName);
        var iban = GenerateIban(bank);
        var account = new Account(accountHolder, accountType, currency, iban);
        bank.OpenAccount(account);
        SaveData();
    }

    public bool Deposit(string iban, decimal amount)
    {
        var account = FindAccount(iban);
        if (account == null) return false;
        
        account.Deposit(amount);
        SaveData();
        return true;
    }

    public bool Withdraw(string iban, decimal amount)
    {
        var account = FindAccount(iban);
        if (account == null) return false;
        
        bool success = account.Withdraw(amount);
        if (success) SaveData();
        return success;
    }

    public bool ChangeCurrency(string iban, AccountCurrency newCurrency, Func<AccountCurrency, AccountCurrency, decimal> rateProvider)
    {
        var account = FindAccount(iban);
        if (account == null) return false;
        
        bool success = account.ChangeCurrency(newCurrency, rateProvider);
        if (success) SaveData();
        return success;
    }

    public bool DeleteAccount(string iban)
    {
        foreach (var bank in _banks)
        {
            if (bank.CloseAccount(iban))
            {
                SaveData();
                return true;
            }
        }
        return false;
    }

    public Account? GetAccount(string iban)
    {
        return FindAccount(iban);
    }

    public List<Account> GetAllAccounts()
    {
        return _banks.SelectMany(b => b.Accounts).ToList();
    }

    public int GetTotalAccountsCount()
    {
        return _banks.Sum(b => b.Accounts.Count);
    }

    public int GetTotalBanksCount()
    {
        return _banks.Count;
    }

    private Account? FindAccount(string iban)
    {
        return _banks.SelectMany(b => b.Accounts)
                     .FirstOrDefault(a => a.Iban == iban);
    }

    private Bank GetOrCreateBank(string bankName)
    {
        var bank = GetBank(bankName);
        if (bank == null)
        {
            string swift = GenerateSwift(bankName);
            bank = new Bank(bankName, swift);
            _banks.Add(bank);
        }
        return bank;
    }

    private string GenerateSwift(string bankName)
    {
        string code = bankName.Length >= 4 
            ? bankName.Substring(0, 4).ToUpper() 
            : bankName.ToUpper().PadRight(4, 'X');
        return code + "ROBU";
    }

    private string GenerateIban(Bank bank)
    {
        var random = new Random();
        var iban = new string[18];
        iban[0] = "DE";
        iban[1] = "44";
        iban[2] = "4";
        iban[3] = "4";
        iban[4] = "4";
        iban[5] = "4";
        iban[6] = "4";
        iban[7] = "4";
        iban[8] = "4";
        iban[9] = "4";
        iban[10] = "4";
        iban[11] = "4";
        iban[12] = "4";
        iban[13] = "4";
        iban[14] = "4";
        iban[15] = "4";
        iban[16] = "4";
        iban[17] = "4";
        iban[18] = "4";
        return string.Join("", iban);
    }

    private void LoadData()
    {
        if (File.Exists(_dataFile))
        {
            var json = File.ReadAllText(_dataFile);
            var banks = JsonSerializer.Deserialize<List<Bank>>(json);
            if (banks != null)
            {
                _banks = banks;
            }
        }
    }

    private void SaveData()
    {
        var json = JsonSerializer.Serialize(_banks);
        File.WriteAllText(_dataFile, json);
    }
}