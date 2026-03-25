using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAMWELLPOS.MVVM.ViewModels
{
    [QueryProperty(nameof(ProductId), "ProductId")]
    public partial class ProductManagementDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty]
        private int _productId;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _category = string.Empty;

        [ObservableProperty]
        private string _priceDisplay = string.Empty;

        [ObservableProperty]
        private string _stockDisplay = string.Empty;

        [ObservableProperty]
        private Color _stockColor = Colors.White;

        [ObservableProperty]
        private string? _imagePath;

        [ObservableProperty]
        private bool _hasImage = false;

        [ObservableProperty]
        private string _productIdDisplay = string.Empty;

        public ProductManagementDetailsViewModel(DatabaseService db)
        {
            _db = db;
        }

        partial void OnProductIdChanged(int value) => _ = LoadProductAsync(value);

        private async Task LoadProductAsync(int id)
        {
            var product = await _db.GetProductById(id);
            if (product is null) return;

            Name = product.Name ?? string.Empty;
            Description = string.IsNullOrWhiteSpace(product.Description)
                ? "No description provided."
                : product.Description;
            Category = string.IsNullOrWhiteSpace(product.Category)
                ? "Uncategorized"
                : product.Category;
            PriceDisplay = product.PriceDisplay;
            StockDisplay = product.StockDisplay;
            StockColor = product.StockColor;
            ImagePath = product.ImagePath;
            HasImage = product.HasImage;
            ProductIdDisplay = $"ID: #{product.Id:D4}";
        }

        [RelayCommand]
        private async Task NavigateToEdit()
        {
            await Shell.Current.GoToAsync($"ProductManagementEdit?ProductId={ProductId}");
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}