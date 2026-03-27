using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;
using System.Collections.ObjectModel;

namespace SAMWELLPOS.MVVM.ViewModels
{
    [QueryProperty(nameof(TotalAmount), "TotalAmount")]
    [QueryProperty(nameof(CashTendered), "CashTendered")]
    [QueryProperty(nameof(ChangeAmount), "ChangeAmount")]
    public partial class POSReceiptViewModel : ObservableObject
    {
        private readonly CartService _cart;

        [ObservableProperty]
        private decimal _totalAmount;

        [ObservableProperty]
        private decimal _cashTendered;

        [ObservableProperty]
        private decimal _changeAmount;

        [ObservableProperty]
        private ObservableCollection<CartItem> _receiptItems = new();

        [ObservableProperty]
        private string _transactionNumber = string.Empty;

        [ObservableProperty]
        private string _transactionDate = string.Empty;

        [ObservableProperty]
        private string _totalDisplay = string.Empty;

        [ObservableProperty]
        private string _cashTenderedDisplay = string.Empty;

        [ObservableProperty]
        private string _changeDisplay = string.Empty;

        [ObservableProperty]
        private string _itemCountDisplay = string.Empty;

        public POSReceiptViewModel(CartService cart)
        {
            _cart = cart;
        }

        public void LoadReceipt(IReadOnlyList<CartItem> snapshot)
        {
            ReceiptItems = new ObservableCollection<CartItem>(snapshot);
            TransactionNumber = $"TXN-{DateTime.Now:yyyyMMddHHmmss}";
            TransactionDate = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
            int totalQty = snapshot.Sum(i => i.Quantity);
            ItemCountDisplay = $"{totalQty} {(totalQty == 1 ? "item" : "items")}";
            TotalDisplay = $"₱{TotalAmount:N2}";
            CashTenderedDisplay = $"₱{CashTendered:N2}";
            ChangeDisplay = $"₱{ChangeAmount:N2}";
        }

        [RelayCommand]
        private async Task NewTransaction()
        {
            await Shell.Current.Navigation.PopToRootAsync();
        }
    }
}