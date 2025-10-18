using System.Windows;
using BankingApp.Models;

namespace BankingApp.Views
{
    public partial class CurrencyChangeDialog : Window
    {
        public CurrencyChangeDialog(AccountCurrency currentCurrency)
        {
            InitializeComponent();
            CurrentCurrencyLabel.Content = $"Current Currency: {currentCurrency}";
        }

        private void OnChangeClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
