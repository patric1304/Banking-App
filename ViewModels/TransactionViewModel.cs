namespace BankingApp.ViewModels;

using BankingApp.Models;
using System.Collections.ObjectModel;

public class TransactionViewModel
{
    public ObservableCollection<Transaction> Transactions { get; set; }
    public Transaction SelectedTransaction { get; set; } = null!;

    public TransactionViewModel()
    {
        Transactions = new ObservableCollection<Transaction>();
    }

    public void LoadTransactions(Account account)
    {
        Transactions.Clear();
        foreach (var transaction in account.Transactions)
        {
            Transactions.Add(transaction);
        }
    }

    public void AddTransaction(Transaction transaction)
    {
        Transactions.Add(transaction);
    }

    public void RemoveTransaction(Transaction transaction)
    {
        Transactions.Remove(transaction);
    }
}