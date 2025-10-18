using System.Windows;
using BankingApp.Models;

namespace BankingApp.Views
{
    public partial class TransactionsDialog : Window
    {
        public TransactionsDialog(Account account)
        {
            InitializeComponent();
            AccountInfoLabel.Text = $"Account: {account.AccountHolder} ({account.Iban})";
            BalanceLabel.Text = $"Current Balance: {account.Amount:F2} {account.AccountCurrency}";
            TransactionsListView.ItemsSource = account.Transactions;
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
