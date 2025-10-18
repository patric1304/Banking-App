using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Effects;
using BankingApp.Models;

namespace BankingApp.Views
{
    public partial class MainWindow : Window
    {
        private List<Bank> _banks = new();
        private readonly string _dataFile = "banks.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            RefreshAccountsList();
        }

        private void ShowDialogWithBlur(Window dialog)
        {
            // Add blur effect to main window
            this.Effect = new BlurEffect { Radius = 10 };
            this.IsEnabled = false;
            
            dialog.Owner = this;
            var result = dialog.ShowDialog();
            
            // Remove blur effect
            this.Effect = null;
            this.IsEnabled = true;
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(_dataFile))
                {
                    string json = File.ReadAllText(_dataFile);
                    _banks = JsonSerializer.Deserialize<List<Bank>>(json) ?? new List<Bank>();
                }
                else
                {
                    _banks = new List<Bank>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _banks = new List<Bank>();
            }
        }

        private void SaveData()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_banks, options);
                File.WriteAllText(_dataFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshAccountsList()
        {
            var allAccounts = _banks.SelectMany(b => b.Accounts).ToList();
            AccountsListView.ItemsSource = null;
            AccountsListView.ItemsSource = allAccounts;
            
            // Update stats
            TotalAccountsText.Text = allAccounts.Count.ToString();
            TotalBanksText.Text = _banks.Count.ToString();
        }

        private void ViewAccounts_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsGroupBox.Visibility == Visibility.Collapsed)
            {
                // Show accounts, hide welcome
                AccountsGroupBox.Visibility = Visibility.Visible;
                WelcomePanel.Visibility = Visibility.Collapsed;
                ViewAccountsButton.Content = "👁️ Hide Accounts";
            }
            else
            {
                // Hide accounts, show welcome
                AccountsGroupBox.Visibility = Visibility.Collapsed;
                WelcomePanel.Visibility = Visibility.Visible;
                ViewAccountsButton.Content = "👁️ View All Accounts";
            }
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddAccountDialog(_banks);
            ShowDialogWithBlur(dialog);
            if (dialog.DialogResult == true)
            {
                var bank = _banks.FirstOrDefault(b => b.Name == dialog.SelectedBankName);
                if (bank == null)
                {
                    // Create bank if it doesn't exist
                    string swift = GenerateSwift(dialog.SelectedBankName);
                    bank = new Bank(dialog.SelectedBankName, swift);
                    _banks.Add(bank);
                }

                string iban = GenerateIban(bank.Swift);
                var account = new Account(dialog.AccountHolder, dialog.AccountType, dialog.AccountCurrency, iban);
                bank.OpenAccount(account);
                RefreshAccountsList();
                SaveData();
            }
        }

        private string GenerateIban(string swift)
        {
            // Generate IBAN: Country Code (RO) + Check Digits (2) + Bank Code (4) + Account Number (16)
            string countryCode = "RO";
            string checkDigits = new Random().Next(10, 99).ToString();
            string bankCode = swift.Length >= 4 ? swift.Substring(0, 4).ToUpper() : swift.PadRight(4, 'X').ToUpper();
            string accountNumber = new Random().Next(1000, 9999).ToString() + 
                                   new Random().Next(1000, 9999).ToString() + 
                                   new Random().Next(1000, 9999).ToString() + 
                                   new Random().Next(1000, 9999).ToString();
            return countryCode + checkDigits + bankCode + accountNumber;
        }

        private string GenerateSwift(string bankName)
        {
            // Generate SWIFT code from bank name: first 4 letters + "RO" + "BU" (Bucharest)
            string letters = new string(bankName.Where(char.IsLetter).Take(4).ToArray()).ToUpper().PadRight(4, 'X');
            return letters + "ROBU";
        }

        private void Deposit_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccount = AccountsListView.SelectedItem as Account;
            if (selectedAccount == null)
            {
                MessageBox.Show("Please select an account first!", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new AmountDialog("Deposit");
            ShowDialogWithBlur(dialog);
            if (dialog.DialogResult == true)
            {
                selectedAccount.Deposit(dialog.Amount);
                RefreshAccountsList();
                SaveData();
            }
        }

        private void Withdraw_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccount = AccountsListView.SelectedItem as Account;
            if (selectedAccount == null)
            {
                MessageBox.Show("Please select an account first!", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new AmountDialog("Withdraw");
            ShowDialogWithBlur(dialog);
            if (dialog.DialogResult == true)
            {
                if (selectedAccount.Withdraw(dialog.Amount))
                {
                    RefreshAccountsList();
                    SaveData();
                }
                else
                {
                    MessageBox.Show("Insufficient funds!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ChangeCurrency_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccount = AccountsListView.SelectedItem as Account;
            if (selectedAccount == null)
            {
                MessageBox.Show("Please select an account first!", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new CurrencyDialog();
            ShowDialogWithBlur(dialog);
            if (dialog.DialogResult == true)
            {
                if (selectedAccount.ChangeCurrency(dialog.SelectedCurrency, GetConversionRate))
                {
                    RefreshAccountsList();
                    SaveData();
                }
                else
                {
                    MessageBox.Show("Currency change failed! Insufficient funds for fee.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ViewTransactions_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccount = AccountsListView.SelectedItem as Account;
            if (selectedAccount == null)
            {
                MessageBox.Show("Please select an account first!", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var transactionWindow = new TransactionWindow(selectedAccount);
            ShowDialogWithBlur(transactionWindow);
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccount = AccountsListView.SelectedItem as Account;
            if (selectedAccount == null)
            {
                MessageBox.Show("Please select an account first!", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete account {selectedAccount.Iban}?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var bank in _banks)
                {
                    if (bank.CloseAccount(selectedAccount.Iban))
                    {
                        RefreshAccountsList();
                        SaveData();
                        break;
                    }
                }
            }
        }

        private decimal GetConversionRate(AccountCurrency from, AccountCurrency to)
        {
            if (from == to) return 1m;

            var rates = new Dictionary<(AccountCurrency, AccountCurrency), decimal>
            {
                { (AccountCurrency.RON, AccountCurrency.EUR), 0.20m },
                { (AccountCurrency.EUR, AccountCurrency.RON), 5.0m },
                { (AccountCurrency.USD, AccountCurrency.EUR), 0.92m },
                { (AccountCurrency.EUR, AccountCurrency.USD), 1.09m },
                { (AccountCurrency.GBP, AccountCurrency.EUR), 1.17m },
                { (AccountCurrency.EUR, AccountCurrency.GBP), 0.85m },
                { (AccountCurrency.USD, AccountCurrency.RON), 4.6m },
                { (AccountCurrency.RON, AccountCurrency.USD), 0.22m }
            };

            return rates.TryGetValue((from, to), out var rate) ? rate : 1m;
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveData();
            base.OnClosed(e);
        }
    }
}