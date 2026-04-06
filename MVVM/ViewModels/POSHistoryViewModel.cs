using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;
using System.Collections.ObjectModel;

namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class POSHistoryViewModel : ObservableObject
    {
        private readonly DatabaseService _db;
        private List<TransactionModel> _allTransactions = new();

        [ObservableProperty]
        private ObservableCollection<TransactionModel> _filteredTransactions = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        public POSHistoryViewModel(DatabaseService db)
        {
            _db = db;
        }

        public async Task LoadHistoryAsync()
        {
            _allTransactions = await _db.GetTransactions();
            ApplyFilter();
        }

        partial void OnSearchTextChanged(string value) => ApplyFilter();

        private void ApplyFilter()
        {
            var query = string.IsNullOrWhiteSpace(SearchText)
                ? _allTransactions
                : _allTransactions.Where(t =>
                    t.TransactionNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    t.CashierFullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            FilteredTransactions = new ObservableCollection<TransactionModel>(query);
        }

        [RelayCommand]
        private async Task NavigateToDetails(int transactionId)
        {
            await Shell.Current.GoToAsync($"POSHistoryDetails?TransactionId={transactionId}");
        }

        [RelayCommand]
        private async Task GoBack() => await Shell.Current.GoToAsync("..");
    }
}