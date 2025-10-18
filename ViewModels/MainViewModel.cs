namespace BankingApp.ViewModels;

using System.Collections.ObjectModel;

public class MainViewModel : BaseViewModel
{
    public ObservableCollection<object> Accounts { get; set; }

    public MainViewModel()
    {
        Accounts = new ObservableCollection<object>();
    }
}