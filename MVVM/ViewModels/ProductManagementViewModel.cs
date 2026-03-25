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
    public partial class ProductManagementViewModel : ObservableObject
    {
        private readonly DatabaseService _db;
        private List<ProductModel> _allProducts = new();

        [ObservableProperty]
        private ObservableCollection<ProductModel> _filteredProducts = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        public ProductManagementViewModel(DatabaseService db)
        {
            _db = db;
        }

        public async Task LoadProductsAsync()
        {
            _allProducts = await _db.GetProducts();
            ApplyFilter();
        }

        partial void OnSearchTextChanged(string value) => ApplyFilter();

        private void ApplyFilter()
        {
            var query = string.IsNullOrWhiteSpace(SearchText)
                ? _allProducts
                : _allProducts.Where(p =>
                    (p.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Category?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Id.ToString().Contains(SearchText)));

            FilteredProducts = new ObservableCollection<ProductModel>(query);
        }

        [RelayCommand]
        private async Task NavigateToCreate()
        {
            await Shell.Current.GoToAsync("ProductManagementCreate");
        }

        [RelayCommand]
        private async Task NavigateToEdit(int productId)
        {
            await Shell.Current.GoToAsync($"ProductManagementEdit?ProductId={productId}");
        }

        [RelayCommand]
        private async Task NavigateToDetails(int productId)
        {
            await Shell.Current.GoToAsync($"ProductManagementDetails?ProductId={productId}");
        }

        [RelayCommand]
        private async Task DeleteProduct(int productId)
        {
            var product = await _db.GetProductById(productId);
            if (product is null) return;

            bool confirmed = await Shell.Current.DisplayAlert(
                "Delete Product",
                $"Are you sure you want to delete \"{product.Name}\"?",
                "Delete", "Cancel");

            if (!confirmed) return;

            await _db.DeleteProduct(product);
            await LoadProductsAsync();
        }
    }
}