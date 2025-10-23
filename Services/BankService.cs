namespace BankingApp.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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

    public async Task CreateAccountAsync(string bankName, string accountHolder, AccountType accountType, AccountCurrency currency)
    {
        var bank = GetOrCreateBank(bankName);
        var iban = GenerateIban(bank);
        var account = new Account(accountHolder, accountType, currency, iban);
        bank.OpenAccount(account);
        await SaveDataAsync();
    }

    public async Task<bool> DepositAsync(string iban, decimal amount)
    {
        var account = FindAccount(iban);
        if (account == null) return false;
        
        account.Deposit(amount);
        await SaveDataAsync();
        return true;
    }

    public async Task<bool> WithdrawAsync(string iban, decimal amount)
    {
        var account = FindAccount(iban);
        if (account == null) return false;
        
        bool success = account.Withdraw(amount);
        if (success) await SaveDataAsync();
        return success;
    }

    public async Task<bool> ChangeCurrencyAsync(string iban, AccountCurrency newCurrency, Func<AccountCurrency, AccountCurrency, decimal> rateProvider)
    {
        var account = FindAccount(iban);
        if (account == null) return false;
        
        bool success = account.ChangeCurrency(newCurrency, rateProvider);
        if (success) await SaveDataAsync();
        return success;
    }

    public async Task<bool> DeleteAccountAsync(string iban)
    {
        foreach (var bank in _banks)
        {
            if (bank.CloseAccount(iban))
            {
                await SaveDataAsync();
                return true;
            }
        }
        return false;
    }

    public async Task<(bool success, string message)> TransferAsync(string fromIban, string toIban, decimal amount)
    {
        var fromAccount = FindAccount(fromIban);
        var toAccount = FindAccount(toIban);

        if (fromAccount == null)
            return (false, "Source account not found!");
        
        if (toAccount == null)
            return (false, "Destination account not found!");
        
        if (fromAccount == toAccount)
            return (false, "Cannot transfer to the same account!");
        
        if (amount <= 0)
            return (false, "Amount must be greater than zero!");
        
        if (fromAccount.Amount < amount)
            return (false, "Insufficient funds!");

        var fromBank = _banks.FirstOrDefault(b => b.Accounts.Contains(fromAccount));
        var toBank = _banks.FirstOrDefault(b => b.Accounts.Contains(toAccount));
        
        bool sameBank = fromBank == toBank;
        
        var random = new Random();
        int delaySeconds = sameBank ? random.Next(1, 11) : random.Next(11, 21);
        string transferType = sameBank ? "same bank" : "different banks";
        
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        
        decimal amountToDeposit = amount;
        
        if (fromAccount.AccountCurrency != toAccount.AccountCurrency)
        {
            var rate = GetConversionRate(fromAccount.AccountCurrency, toAccount.AccountCurrency);
            amountToDeposit = amount * rate * 0.99m;
        }
        
        if (!fromAccount.Withdraw(amount))
            return (false, "Withdrawal failed!");
        
        toAccount.Deposit(amountToDeposit);
        
        await SaveDataAsync();
        
        return (true, "Transfer completed successfully!");
    }

    private decimal GetConversionRate(AccountCurrency from, AccountCurrency to)
    {
        if (from == to) return 1.0m;
        
        if (from == AccountCurrency.RON && to == AccountCurrency.EUR) return 0.20m;
        if (from == AccountCurrency.RON && to == AccountCurrency.USD) return 0.22m;
        if (from == AccountCurrency.RON && to == AccountCurrency.GBP) return 0.17m;
        
        if (from == AccountCurrency.EUR && to == AccountCurrency.RON) return 5.00m;
        if (from == AccountCurrency.EUR && to == AccountCurrency.USD) return 1.10m;
        if (from == AccountCurrency.EUR && to == AccountCurrency.GBP) return 0.85m;
        
        if (from == AccountCurrency.USD && to == AccountCurrency.RON) return 4.50m;
        if (from == AccountCurrency.USD && to == AccountCurrency.EUR) return 0.91m;
        if (from == AccountCurrency.USD && to == AccountCurrency.GBP) return 0.77m;
        
        if (from == AccountCurrency.GBP && to == AccountCurrency.RON) return 5.90m;
        if (from == AccountCurrency.GBP && to == AccountCurrency.EUR) return 1.18m;
        if (from == AccountCurrency.GBP && to == AccountCurrency.USD) return 1.30m;
        
        return 1.0m;
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
        var checkDigits = random.Next(10, 100);
        var bankCode = bank.Swift.Substring(0, 4);
        var accountNumber = random.Next(1000000000, int.MaxValue).ToString().PadLeft(16, '0');
        return $"RO{checkDigits}{bankCode}{accountNumber}";
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

    private async Task SaveDataAsync()
    {
        var json = JsonSerializer.Serialize(_banks, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_dataFile, json);
    }
}