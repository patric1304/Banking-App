using System.Windows;
using BankingApp.ViewModels;

namespace BankingApp.Views
{
    public partial class TransactionView : Window
    {
        public TransactionView()
        {
            InitializeComponent();
            DataContext = new TransactionViewModel();
        }
    }
}