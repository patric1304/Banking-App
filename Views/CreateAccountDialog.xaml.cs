using System.Windows;

namespace BankingApp.Views
{
    public partial class CreateAccountDialog : Window
    {
        public CreateAccountDialog()
        {
            InitializeComponent();
        }

        private void OnCreateClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AccountHolderInput.Text))
            {
                MessageBox.Show("Please enter account holder name.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
