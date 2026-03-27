using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private List<ProductModel> _allProducts = new();

        [ObservableProperty]
        private ObservableCollection<ProductModel> _filteredProducts = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private int _cartCount = 0;

        [ObservableProperty]
        private string _selectedCategory = "All";

        public List<string> Categories { get; private set; } = new();

        public POSDashboardViewModel(DatabaseService db, CartService cart)
        {
            _db = db;
            _cart = cart;

            // Subscribe to cart changes to update badge
            _cart.CartChanged += OnCartChanged;
        }

        private void OnCartChanged()
        {
            CartCount = _cart.TotalItemCount;
            // Refresh to update stepper quantities on cards
            ApplyFilter();
        }

        public async Task LoadProductsAsync()
        {
            // Only load in-stock products for cashier
            var all = await _db.GetProducts();
            _allProducts = all.Where(p => p.Quantity > 0).ToList();

            // Build category list dynamically from products
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

            // Stamp cart quantities onto each product for the stepper display
            var list = query.ToList();
            foreach (var p in list)
                p.CartQuantity = _cart.GetQuantity(p);

            FilteredProducts = new ObservableCollection<ProductModel>(list);
        }

        // Called by + button on each card
        [RelayCommand]
        private void Increment(ProductModel product)
        {
            // Don't exceed available stock
            int inCart = _cart.GetQuantity(product);
            if (inCart >= product.Quantity) return;
            _cart.AddOrIncrement(product);
        }

        // Called by - button on each card
        [RelayCommand]
        private void Decrement(ProductModel product)
        {
            _cart.Decrement(product);
        }

        // Returns cart quantity for a product — used by card display
        public int GetCartQuantity(ProductModel product)
            => _cart.GetQuantity(product);

        [RelayCommand]
        private async Task GoToCart()
        {
            await Shell.Current.GoToAsync("POSCart");
        }

        [RelayCommand]
        private async Task Logout()
        {
            bool confirmed = await Shell.Current.DisplayAlert(
                "Logout", "Are you sure you want to log out?", "Logout", "Cancel");
            if (!confirmed) return;

            _cart.Clear();

            _cart.Clear();
            App.Current!.MainPage = new AppShell();
        }

        [RelayCommand]
        private void SetCategory(string category)
        {
            SelectedCategory = category;
        }
    }
}
