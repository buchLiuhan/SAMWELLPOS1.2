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
    public partial class ProductManagementEditViewModel : ObservableObject
    {
        private readonly DatabaseService _db;
        private ProductModel? _originalProduct;

        [ObservableProperty]
        private int _productId;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _price = string.Empty;

        [ObservableProperty]
        private string _quantity = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        public ProductManagementEditViewModel(DatabaseService db)
        {
            _db = db;
        }

        partial void OnProductIdChanged(int value) => _ = LoadProductAsync(value);

        private async Task LoadProductAsync(int id)
        {
            _originalProduct = await _db.GetProductById(id);
            if (_originalProduct is null) return;

            Name = _originalProduct.Name ?? string.Empty;
            Price = _originalProduct.Price.ToString("F2");
            Quantity = _originalProduct.Quantity.ToString();
        }

        [RelayCommand]
        private async Task SaveChanges()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = "Product name is required.";
                HasError = true;
                return;
            }

            if (!decimal.TryParse(Price, out decimal parsedPrice) || parsedPrice < 0)
            {
                ErrorMessage = "Please enter a valid price.";
                HasError = true;
                return;
            }

            if (!int.TryParse(Quantity, out int parsedQty) || parsedQty < 0)
            {
                ErrorMessage = "Please enter a valid quantity.";
                HasError = true;
                return;
            }

            if (_originalProduct is null) return;

            _originalProduct.Name = Name.Trim();
            _originalProduct.Price = parsedPrice;
            _originalProduct.Quantity = parsedQty;

            await _db.UpdateProduct(_originalProduct);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}