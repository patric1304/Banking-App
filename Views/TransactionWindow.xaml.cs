using System.Windows;
using BankingApp.Models;

namespace BankingApp.Views
{
    public partial class TransactionWindow : Window
    {
        public TransactionWindow(Account account)
        {
            InitializeComponent();
            AccountInfoTextBlock.Text = $"Transactions for {account.AccountHolder} ({account.Iban})";
            TransactionsListView.ItemsSource = account.Transactions;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
