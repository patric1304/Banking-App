using System.Windows;

namespace BankingApp.Views
{
    public partial class AddBankDialog : Window
    {
        public string BankName { get; private set; } = string.Empty;
        public string Swift { get; private set; } = string.Empty;

        public AddBankDialog()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BankNameTextBox.Text))
            {
                MessageBox.Show("Please enter a bank name!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(SwiftTextBox.Text))
            {
                MessageBox.Show("Please enter a Swift code!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BankName = BankNameTextBox.Text.Trim();
            Swift = SwiftTextBox.Text.Trim();
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
