using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;
using System.Collections.ObjectModel;
using System.Transactions;

namespace SAMWELLPOS.MVVM.ViewModels
{
    [QueryProperty(nameof(TransactionId), "TransactionId")]
    public partial class POSHistoryDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty]
        private int _transactionId;

        [ObservableProperty]
        private string _transactionNumber = string.Empty;

        [ObservableProperty]
        private string _cashierFullName = string.Empty;

        [ObservableProperty]
        private string _transactionDate = string.Empty;

        [ObservableProperty]
        private string _totalDisplay = string.Empty;

        [ObservableProperty]
        private string _cashTenderedDisplay = string.Empty;

        [ObservableProperty]
        private string _changeDisplay = string.Empty;

        [ObservableProperty]
        private ObservableCollection<TransactionItemModel> _items = new();

        public POSHistoryDetailsViewModel(DatabaseService db)
        {
            _db = db;
        }

        partial void OnTransactionIdChanged(int value) => _ = LoadDetailsAsync(value);

        private async Task LoadDetailsAsync(int id)
        {
            var txn = await _db.GetTransactionById(id);
            if (txn is null) return;

            TransactionNumber = txn.TransactionNumber;
            CashierFullName = txn.CashierFullName;
            TransactionDate = txn.DateDisplay;
            TotalDisplay = txn.TotalDisplay;
            CashTenderedDisplay = $"₱{txn.CashTendered:N2}";
            ChangeDisplay = $"₱{txn.ChangeAmount:N2}";

            var itemList = await _db.GetTransactionItems(id);
            Items = new ObservableCollection<TransactionItemModel>(itemList);
        }

        [RelayCommand]
        private async Task GoBack() => await Shell.Current.GoToAsync("..");
    }
}