using System;
using System.Windows;

namespace BankingApp.Views
{
    public partial class AmountDialog : Window
    {
        public decimal Amount { get; private set; }

        public AmountDialog(string title)
        {
            InitializeComponent();
            Title = title;
            TitleTextBlock.Text = $"{title} Amount:";
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text, out decimal amount) && amount > 0)
            {
                Amount = amount;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid positive amount!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
