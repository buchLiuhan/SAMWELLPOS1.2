using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;
using System.Collections.ObjectModel;

namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class POSDashboardViewModel : ObservableObject
    {
        private readonly DatabaseService _db;
        private readonly CartService _cart;
        private readonly SessionService _session;
        private List<ProductModel> _allProducts = new();

        [ObservableProperty]
        private ObservableCollection<ProductModel> _filteredProducts = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private int _cartCount = 0;

        [ObservableProperty]
        private string _selectedCategory = "All";

        // Cashier info
        public string CashierFullName => _session.CurrentUser?.FullName ?? "Cashier";
        public string? CashierProfilePicturePath => _session.CurrentUser?.ProfilePicturePath;
        public bool CashierHasPhoto => !string.IsNullOrEmpty(_session.CurrentUser?.ProfilePicturePath);
        public string CashierInitials
        {
            get
            {
                var name = _session.CurrentUser?.FullName;
                if (string.IsNullOrWhiteSpace(name)) return "?";
                var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return parts.Length >= 2
                    ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                    : parts[0][0].ToString().ToUpper();
            }
        }

        public List<string> Categories { get; private set; } = new();

        public POSDashboardViewModel(DatabaseService db, CartService cart, SessionService session)
        {
            _db = db;
            _cart = cart;
            _session = session;
            _cart.CartChanged += OnCartChanged;
        }

        private void OnCartChanged()
        {
            CartCount = _cart.TotalItemCount;
            ApplyFilter();
        }

        public async Task LoadProductsAsync()
        {
            var all = await _db.GetProducts();
            _allProducts = all.Where(p => p.Quantity > 0).ToList();

            var cats = _allProducts
                .Where(p => !string.IsNullOrWhiteSpace(p.Category))
                .Select(p => p.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            Categories = new List<string> { "All" }.Concat(cats).ToList();
            OnPropertyChanged(nameof(Categories));
            ApplyFilter();
        }

        partial void OnSearchTextChanged(string value) => ApplyFilter();
        partial void OnSelectedCategoryChanged(string value) => ApplyFilter();

        private void ApplyFilter()
        {
            var query = _allProducts.AsEnumerable();

            if (SelectedCategory != "All")
                query = query.Where(p => p.Category == SelectedCategory);

            if (!string.IsNullOrWhiteSpace(SearchText))
                query = query.Where(p =>
                    (p.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Id.ToString().Contains(SearchText)));

            var list = query.ToList();
            foreach (var p in list)
                p.CartQuantity = _cart.GetQuantity(p);

            FilteredProducts = new ObservableCollection<ProductModel>(list);
        }

        [RelayCommand]
        private void Increment(ProductModel product)
        {
            int inCart = _cart.GetQuantity(product);
            if (inCart >= product.Quantity) return;
            _cart.AddOrIncrement(product);
        }

        [RelayCommand]
        private void Decrement(ProductModel product)
        {
            _cart.Decrement(product);
        }

        public int GetCartQuantity(ProductModel product)
            => _cart.GetQuantity(product);

        [RelayCommand]
        private async Task GoToCart()
        {
            await Shell.Current.GoToAsync("POSCart");
        }

        [RelayCommand]
        private async Task GoToHistory()
        {
            await Shell.Current.GoToAsync("POSHistory");
        }

        [RelayCommand]
        private async Task Logout()
        {
            bool confirmed = await Shell.Current.DisplayAlert(
                "Logout", "Are you sure you want to log out?", "Logout", "Cancel");
            if (!confirmed) return;

            _cart.Clear();
            _session.Clear();
            App.Current!.MainPage = new AppShell();
        }

        [RelayCommand]
        private void SetCategory(string category)
        {
            SelectedCategory = category;
        }

        [RelayCommand]
        private async Task PromptQuantity(ProductModel product)
        {
            string? result = await Shell.Current.DisplayPromptAsync(
                "Set Quantity",
                $"{product.Name} (Max: {product.Quantity})",
                accept: "Set",
                cancel: "Cancel",
                placeholder: "Enter quantity",
                maxLength: 4,
                keyboard: Keyboard.Numeric,
                initialValue: _cart.GetQuantity(product).ToString());

            if (result is null) return;
            if (!int.TryParse(result, out int qty) || qty < 0) return;

            qty = Math.Min(qty, product.Quantity); // cap at available stock

            int current = _cart.GetQuantity(product);
            int diff = qty - current;

            if (diff > 0)
                for (int i = 0; i < diff; i++) _cart.AddOrIncrement(product);
            else if (diff < 0)
                for (int i = 0; i < Math.Abs(diff); i++) _cart.Decrement(product);
        }







    }
}