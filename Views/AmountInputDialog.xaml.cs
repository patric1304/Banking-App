using System.Windows;

namespace BankingApp.Views
{
    public partial class AmountInputDialog : Window
    {
        public decimal Amount { get; private set; }

        public AmountInputDialog(string title, string prompt)
        {
            InitializeComponent();
            Title = title;
            PromptLabel.Content = prompt;
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountInput.Text, out decimal amount) && amount > 0)
            {
                Amount = amount;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter a valid positive amount.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
