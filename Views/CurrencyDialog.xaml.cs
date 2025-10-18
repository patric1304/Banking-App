using System;
using System.Windows;
using BankingApp.Models;

namespace BankingApp.Views
{
    public partial class CurrencyDialog : Window
    {
        public AccountCurrency SelectedCurrency { get; private set; }

        public CurrencyDialog()
        {
            InitializeComponent();
            CurrencyComboBox.ItemsSource = Enum.GetValues(typeof(AccountCurrency));
            CurrencyComboBox.SelectedIndex = 0;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (CurrencyComboBox.SelectedItem != null)
            {
                SelectedCurrency = (AccountCurrency)CurrencyComboBox.SelectedItem;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a currency!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
