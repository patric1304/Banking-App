using System;
using System.Collections.Generic;
using System.Windows;
using BankingApp.Models;

namespace BankingApp.Views
{
    public partial class AddAccountDialog : Window
    {
        public string AccountHolder { get; private set; } = string.Empty;
        public AccountType AccountType { get; private set; }
        public AccountCurrency AccountCurrency { get; private set; }
        public string SelectedBankName { get; private set; } = string.Empty;

        public AddAccountDialog(List<Bank> banks)
        {
            InitializeComponent();

            // Populate account types
            AccountTypeComboBox.ItemsSource = Enum.GetValues(typeof(AccountType));
            AccountTypeComboBox.SelectedIndex = 0;

            // Populate currencies
            CurrencyComboBox.ItemsSource = Enum.GetValues(typeof(AccountCurrency));
            CurrencyComboBox.SelectedIndex = 0;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BankNameTextBox.Text))
            {
                MessageBox.Show("Please enter a bank name!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(AccountHolderTextBox.Text))
            {
                MessageBox.Show("Please enter an account holder name!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedBankName = BankNameTextBox.Text.Trim();
            AccountHolder = AccountHolderTextBox.Text.Trim();
            AccountType = (AccountType)AccountTypeComboBox.SelectedItem;
            AccountCurrency = (AccountCurrency)CurrencyComboBox.SelectedItem;

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
