
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;

namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class POSPaymentViewModel : ObservableObject
    {
        private readonly CartService _cart;
        private readonly DatabaseService _db;
        private IReadOnlyList<CartItem> _snapshot = new List<CartItem>();

        [ObservableProperty]
        private string _totalDisplay = "₱0.00";

        [ObservableProperty]
        private decimal _totalAmount = 0;

        [ObservableProperty]
        private string _cashTendered = string.Empty;

        [ObservableProperty]
        private string _changeDisplay = "₱0.00";

        [ObservableProperty]
        private bool _isInsufficient = false;

        [ObservableProperty]
        private bool _hasError = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public POSPaymentViewModel(CartService cart, DatabaseService db)
        {
            _cart = cart;
            _db = db;
        }

        public void LoadTotal()
        {
            // Snapshot cart items before anything clears them
            _snapshot = _cart.Items.ToList();
            TotalAmount = _cart.TotalAmount;
            TotalDisplay = $"₱{TotalAmount:N2}";
            CashTendered = string.Empty;
            ChangeDisplay = "₱0.00";
            IsInsufficient = false;
            HasError = false;
        }

        // Live change calculation as user types
        partial void OnCashTenderedChanged(string value)
        {
            HasError = false;
            IsInsufficient = false;

            if (string.IsNullOrWhiteSpace(value) ||
                !decimal.TryParse(value, out decimal cash) || cash < 0)
            {
                ChangeDisplay = "₱0.00";
                return;
            }

            decimal change = cash - TotalAmount;

            if (change < 0)
            {
                ChangeDisplay = $"₱{Math.Abs(change):N2} short";
                IsInsufficient = true;
            }
            else
            {
                ChangeDisplay = $"₱{change:N2}";
                IsInsufficient = false;
            }
        }

        // Quick cash preset buttons
        [RelayCommand]
        private void SetAmount(string amount)
        {
            CashTendered = amount;
        }

        [RelayCommand]
        private void SetExactAmount()
        {
            CashTendered = TotalAmount.ToString("F2");
        }

        [RelayCommand]
        private async Task ConfirmPayment()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(CashTendered))
            {
                ErrorMessage = "Please enter the cash amount tendered.";
                HasError = true;
                return;
            }

            if (!decimal.TryParse(CashTendered, out decimal cash) || cash <= 0)
            {
                ErrorMessage = "Please enter a valid cash amount.";
                HasError = true;
                return;
            }

            if (cash < TotalAmount)
            {
                ErrorMessage = "Cash tendered is less than the total amount due.";
                HasError = true;
                return;
            }

            decimal change = cash - TotalAmount;

            // Deduct stock from DB
            foreach (var item in _snapshot)
            {
                var product = await _db.GetProductById(item.Product.Id);
                if (product is null) continue;
                product.Quantity -= item.Quantity;
                if (product.Quantity < 0) product.Quantity = 0;
                await _db.UpdateProduct(product);
            }

            // Clear cart
            _cart.Clear();

            // Navigate to receipt
            await Shell.Current.GoToAsync(
                $"POSReceipt?TotalAmount={TotalAmount}&CashTendered={cash}&ChangeAmount={change}");
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}