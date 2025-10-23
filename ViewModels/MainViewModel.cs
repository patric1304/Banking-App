namespace BankingApp.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BankingApp.Helpers;
using BankingApp.Models;
using BankingApp.Services;

public class MainViewModel : BaseViewModel
{
    private readonly BankService _bankService;
    private bool _isAccountsVisible;

    public ObservableCollection<Account> Accounts { get; set; }
    
    public int TotalAccounts => _bankService.GetTotalAccountsCount();
    public int TotalBanks => _bankService.GetTotalBanksCount();
    
    public bool IsAccountsVisible
    {
        get => _isAccountsVisible;
        set
        {
            _isAccountsVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsWelcomePanelVisible));
        }
    }

    public bool IsWelcomePanelVisible => !_isAccountsVisible;

    public ICommand ViewAccountsCommand { get; }
    public ICommand CreateAccountCommand { get; }
    public ICommand DepositCommand { get; }
    public ICommand WithdrawCommand { get; }
    public ICommand ChangeCurrencyCommand { get; }
    public ICommand ViewTransactionsCommand { get; }
    public ICommand DeleteAccountCommand { get; }

    public MainViewModel()
    {
        _bankService = new BankService();
        Accounts = new ObservableCollection<Account>();
        
        LoadAccounts();
        
        ViewAccountsCommand = new RelayCommand(ToggleAccountsView);
        CreateAccountCommand = new RelayCommand(CreateAccount);
        DepositCommand = new RelayCommand(Deposit, CanExecuteAccountAction);
        WithdrawCommand = new RelayCommand(Withdraw, CanExecuteAccountAction);
        ChangeCurrencyCommand = new RelayCommand(ChangeCurrency, CanExecuteAccountAction);
        ViewTransactionsCommand = new RelayCommand(ViewTransactions, CanExecuteAccountAction);
        DeleteAccountCommand = new RelayCommand(DeleteAccount, CanExecuteAccountAction);
        
        IsAccountsVisible = false;
    }

    private void ToggleAccountsView(object parameter)
    {
        IsAccountsVisible = !IsAccountsVisible;
    }

    private void CreateAccount(object parameter)
    {
        // Pass the list of banks to the dialog constructor
        var dialog = new Views.AddAccountDialog(_bankService.GetAllBanks())
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        if (dialog.ShowDialog() == true)
        {
            _bankService.CreateAccount(
                dialog.SelectedBankName,
                dialog.AccountHolder,
                dialog.AccountType,
                dialog.AccountCurrency
            );
            RefreshAccounts();
        }
    }

    private bool CanExecuteAccountAction(object parameter)
    {
        return parameter is Account;
    }

    private void Deposit(object parameter)
    {
        if (parameter is not Account account) return;

        var dialog = new Views.AmountDialog("Deposit")
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        if (dialog.ShowDialog() == true)
        {
            if (_bankService.Deposit(account.Iban, dialog.Amount))
            {
                RefreshAccounts();
                MessageBox.Show($"Successfully deposited {dialog.Amount} {account.AccountCurrency}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    private void Withdraw(object parameter)
    {
        if (parameter is not Account account) return;

        var dialog = new Views.AmountDialog("Withdraw")
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        if (dialog.ShowDialog() == true)
        {
            if (_bankService.Withdraw(account.Iban, dialog.Amount))
            {
                RefreshAccounts();
                MessageBox.Show($"Successfully withdrew {dialog.Amount} {account.AccountCurrency}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Insufficient funds!", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ChangeCurrency(object parameter)
    {
        if (parameter is not Account account) return;

        // CurrencyDialog doesn't take parameters - create it without arguments
        var dialog = new Views.CurrencyDialog()
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        if (dialog.ShowDialog() == true)
        {
            if (_bankService.ChangeCurrency(account.Iban, dialog.SelectedCurrency, GetConversionRate))
            {
                RefreshAccounts();
                MessageBox.Show($"Currency changed to {dialog.SelectedCurrency}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Insufficient funds for currency change!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ViewTransactions(object parameter)
    {
        if (parameter is not Account account) return;

        var window = new Views.TransactionWindow(account)
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        window.ShowDialog();
    }

    private void DeleteAccount(object parameter)
    {
        if (parameter is not Account account) return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete account {account.Iban}?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );

        if (result == MessageBoxResult.Yes)
        {
            if (_bankService.DeleteAccount(account.Iban))
            {
                RefreshAccounts();
                MessageBox.Show("Account deleted successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    private void LoadAccounts()
    {
        Accounts.Clear();
        foreach (var account in _bankService.GetAllAccounts())
        {
            Accounts.Add(account);
        }
    }

    private void RefreshAccounts()
    {
        LoadAccounts();
        OnPropertyChanged(nameof(TotalAccounts));
        OnPropertyChanged(nameof(TotalBanks));
    }

    private decimal GetConversionRate(AccountCurrency from, AccountCurrency to)
    {
        if (from == to) return 1m;

        var rates = new System.Collections.Generic.Dictionary<(AccountCurrency, AccountCurrency), decimal>
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