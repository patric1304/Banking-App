namespace BankingApp.ViewModels;

using BankingApp.Helpers;
using BankingApp.Models;
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
        // Implement currency conversion logic here
        return 1.0m; // Placeholder for conversion rate
    }
}