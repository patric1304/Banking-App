namespace BankingApp.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BankingApp.Helpers;
using BankingApp.Models;
using BankingApp.Services;

public class MainViewModel : BaseViewModel
{
    private readonly BankService _bankService;
    private bool _isAccountsVisible;
    private bool _isLoading;

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

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public ICommand ViewAccountsCommand { get; }
    public ICommand CreateAccountCommand { get; }
    public ICommand DepositCommand { get; }
    public ICommand WithdrawCommand { get; }
    public ICommand ChangeCurrencyCommand { get; }
    public ICommand ViewTransactionsCommand { get; }
    public ICommand DeleteAccountCommand { get; }
    public ICommand TransferCommand { get; }

    public MainViewModel()
    {
        _bankService = new BankService();
        Accounts = new ObservableCollection<Account>();
        
        LoadAccounts();
        
        ViewAccountsCommand = new RelayCommand(ToggleAccountsView);
        CreateAccountCommand = new RelayCommand(async _ => await CreateAccountAsync());
        DepositCommand = new RelayCommand(async param => await DepositAsync(param), CanExecuteAccountAction);
        WithdrawCommand = new RelayCommand(async param => await WithdrawAsync(param), CanExecuteAccountAction);
        ChangeCurrencyCommand = new RelayCommand(async param => await ChangeCurrencyAsync(param), CanExecuteAccountAction);
        ViewTransactionsCommand = new RelayCommand(ViewTransactions, CanExecuteAccountAction);
        DeleteAccountCommand = new RelayCommand(async param => await DeleteAccountAsync(param), CanExecuteAccountAction);
        TransferCommand = new RelayCommand(async param => await TransferAsync(param), CanExecuteAccountAction);
        
        IsAccountsVisible = false;
    }

    private void ToggleAccountsView(object parameter)
    {
        IsAccountsVisible = !IsAccountsVisible;
    }

    private async Task CreateAccountAsync()
    {
        var dialog = new Views.AddAccountDialog(_bankService.GetAllBanks())
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        
        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            try
            {
                await _bankService.CreateAccountAsync(
                    dialog.SelectedBankName,
                    dialog.AccountHolder,
                    dialog.AccountType,
                    dialog.AccountCurrency
                );
                await RefreshAccountsAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private bool CanExecuteAccountAction(object parameter)
    {
        return parameter is Account && !IsLoading;
    }

    private async Task DepositAsync(object parameter)
    {
        if (parameter is not Account account) return;

        var dialog = new Views.AmountDialog("Deposit")
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        
        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            try
            {
                if (await _bankService.DepositAsync(account.Iban, dialog.Amount))
                {
                    await RefreshAccountsAsync();
                    MessageBox.Show($"Successfully deposited {dialog.Amount} {account.AccountCurrency}",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private async Task WithdrawAsync(object parameter)
    {
        if (parameter is not Account account) return;

        var dialog = new Views.AmountDialog("Withdraw")
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        
        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            try
            {
                if (await _bankService.WithdrawAsync(account.Iban, dialog.Amount))
                {
                    await RefreshAccountsAsync();
                    MessageBox.Show($"Successfully withdrew {dialog.Amount} {account.AccountCurrency}",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Insufficient funds!", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private async Task ChangeCurrencyAsync(object parameter)
    {
        if (parameter is not Account account) return;

        var dialog = new Views.CurrencyDialog()
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        
        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            try
            {
                if (await _bankService.ChangeCurrencyAsync(account.Iban, dialog.SelectedCurrency, GetConversionRate))
                {
                    await RefreshAccountsAsync();
                    MessageBox.Show($"Currency changed to {dialog.SelectedCurrency}",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Insufficient funds for currency change!",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                IsLoading = false;
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

    private async Task DeleteAccountAsync(object parameter)
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
            IsLoading = true;
            try
            {
                if (await _bankService.DeleteAccountAsync(account.Iban))
                {
                    await RefreshAccountsAsync();
                    MessageBox.Show("Account deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private async Task TransferAsync(object parameter)
    {
        if (parameter is not Account fromAccount)
            return;

        var allAccounts = _bankService.GetAllAccounts();
        if (allAccounts.Count < 2)
        {
            MessageBox.Show("You need at least two accounts to make a transfer!", "Transfer Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dialog = new Views.TransferDialog(allAccounts, fromAccount)
        {
            Owner = Application.Current.MainWindow
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                IsLoading = true;
                
                var (success, message) = await _bankService.TransferAsync(
                    fromAccount.Iban,
                    dialog.SelectedToAccount!.Iban,
                    dialog.Amount);

                if (success)
                {
                    await RefreshAccountsAsync();
                    MessageBox.Show(message, "Transfer Successful",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(message, "Transfer Failed",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                IsLoading = false;
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

    private async Task RefreshAccountsAsync()
    {
        await Task.Run(() =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LoadAccounts();
                OnPropertyChanged(nameof(TotalAccounts));
                OnPropertyChanged(nameof(TotalBanks));
            });
        });
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