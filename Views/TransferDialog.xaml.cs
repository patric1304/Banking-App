using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BankingApp.Models;

namespace BankingApp.Views
{
    public partial class TransferDialog : Window
    {
        public Account? SelectedToAccount => ToAccountComboBox.SelectedItem as Account;
        public decimal Amount { get; private set; }

        public TransferDialog(List<Account> allAccounts, Account fromAccount)
        {
            InitializeComponent();

            FromAccountTextBox.Text = $"{fromAccount.Iban} ({fromAccount.AccountHolder}) - {fromAccount.Amount:F2} {fromAccount.AccountCurrency}";

            var destinationAccounts = allAccounts.Where(a => a != fromAccount).ToList();
            ToAccountComboBox.ItemsSource = destinationAccounts;

            if (destinationAccounts.Count > 0)
                ToAccountComboBox.SelectedIndex = 0;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedToAccount == null)
            {
                MessageBox.Show("Please select a destination account!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(AmountTextBox.Text) || !decimal.TryParse(AmountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid positive amount!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Amount = amount;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
