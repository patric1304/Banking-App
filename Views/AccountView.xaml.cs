using System.Windows;
using BankingApp.Models;
using BankingApp.ViewModels;

namespace BankingApp.Views
{
    public partial class AccountView : Window
    {
        public AccountView(Account account)
        {
            InitializeComponent();
            DataContext = new AccountViewModel(account);
        }
    }
}