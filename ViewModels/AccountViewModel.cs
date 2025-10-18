namespace BankingApp.ViewModels;

using BankingApp.Helpers;
using BankingApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class AccountViewModel : BaseViewModel
{
    private Account _account = null!;
    public Account Account
    {
        get => _account;
        set
        {
            _account = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Transaction> Transactions => new ObservableCollection<Transaction>(Account.Transactions);

    public ICommand DepositCommand { get; }
    public ICommand WithdrawCommand { get; }
    public ICommand ChangeCurrencyCommand { get; }

    public AccountViewModel(Account account)
    {
        Account = account;
        DepositCommand = new RelayCommand(Deposit);
        WithdrawCommand = new RelayCommand(Withdraw);
        ChangeCurrencyCommand = new RelayCommand(ChangeCurrency);
    }

    private void Deposit(object parameter)
    {
        if (parameter is decimal amount && amount > 0)
        {
            Account.Deposit(amount);
            OnPropertyChanged(nameof(Transactions));
        }
    }

    private void Withdraw(object parameter)
    {
        if (parameter is decimal amount && amount > 0)
        {
            Account.Withdraw(amount);
            OnPropertyChanged(nameof(Transactions));
        }
    }

    private void ChangeCurrency(object parameter)
    {
        if (parameter is AccountCurrency newCurrency)
        {
            // Assume rateProvider is defined elsewhere
            Account.ChangeCurrency(newCurrency, RateProvider);
            OnPropertyChanged(nameof(Transactions));
        }
    }

    private decimal RateProvider(AccountCurrency from, AccountCurrency to)
    {
        if (from == to) return 1m;

        var rates = new Dictionary<(AccountCurrency, AccountCurrency), decimal>
        {
            { (AccountCurrency.RON, AccountCurrency.EUR), 0.20m },
            { (AccountCurrency.EUR, AccountCurrency.RON), 5.0m },
            { (AccountCurrency.USD, AccountCurrency.EUR), 0.92m },
            { (AccountCurrency.EUR, AccountCurrency.USD), 1.09m },
            { (AccountCurrency.GBP, AccountCurrency.EUR), 1.17m },
            { (AccountCurrency.EUR, AccountCurrency.GBP), 0.85m },
            { (AccountCurrency.USD, AccountCurrency.RON), 4.6m },
            { (AccountCurrency.RON, AccountCurrency.USD), 0.22m },
            { (AccountCurrency.GBP, AccountCurrency.RON), 5.85m },
            { (AccountCurrency.RON, AccountCurrency.GBP), 0.17m },
            { (AccountCurrency.USD, AccountCurrency.GBP), 0.78m },
            { (AccountCurrency.GBP, AccountCurrency.USD), 1.28m }
        };

        return rates.TryGetValue((from, to), out var rate) ? rate : 1m;
    }
}