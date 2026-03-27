using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;
using System.Collections.ObjectModel;

namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class POSCartViewModel : ObservableObject
    {
        private readonly CartService _cart;
        private readonly DatabaseService _db;

        [ObservableProperty]
        private ObservableCollection<CartItem> _cartItems = new();

        [ObservableProperty]
        private string _subtotalDisplay = "₱0.00";

        [ObservableProperty]
        private string _totalDisplay = "₱0.00";

        [ObservableProperty]
        private string _itemCountDisplay = "0 items";

        [ObservableProperty]
        private bool _hasItems = false;

        public POSCartViewModel(CartService cart, DatabaseService db)
        {
            _cart = cart;
            _db = db;
            _cart.CartChanged += RefreshCart;
        }

        public void RefreshCart()
        {
            CartItems = new ObservableCollection<CartItem>(_cart.Items);
            HasItems = CartItems.Count > 0;
            int totalQty = _cart.TotalItemCount;
            decimal total = _cart.TotalAmount;
            ItemCountDisplay = $"{totalQty} {(totalQty == 1 ? "item" : "items")}";
            SubtotalDisplay = $"₱{total:N2}";
            TotalDisplay = $"₱{total:N2}";
        }

        [RelayCommand]
        private void Increment(CartItem item)
        {
            int inCart = _cart.GetQuantity(item.Product);
            if (inCart >= item.Product.Quantity) return;
            _cart.AddOrIncrement(item.Product);
        }

        [RelayCommand]
        private void Decrement(CartItem item)
        {
            _cart.Decrement(item.Product);
        }

        [RelayCommand]
        private async Task ClearCart()
        {
            if (!HasItems) return;

            bool confirmed = await Shell.Current.DisplayAlert(
                "Clear Cart",
                "Are you sure you want to remove all items?",
                "Clear", "Cancel");

            if (!confirmed) return;
            _cart.Clear();
        }

        [RelayCommand]
        private async Task CompletePayment()
        {
            if (!HasItems)
            {
                await Shell.Current.DisplayAlert(
                    "Empty Cart",
                    "Please add items before completing payment.",
                    "OK");
                return;
            }

            await Shell.Current.GoToAsync("POSPayment");
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}