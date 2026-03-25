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
    public partial class ProductManagementCreateViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _category = string.Empty;

        [ObservableProperty]
        private string _price = string.Empty;

        [ObservableProperty]
        private string _quantity = string.Empty;

        [ObservableProperty]
        private string? _imagePath;

        [ObservableProperty]
        private bool _hasImage = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        public List<string> Categories { get; } = new()
        {
            "Food", "Drinks", "Electronics", "Clothing", "Accessories", "Other"
        };

        public ProductManagementCreateViewModel(DatabaseService db)
        {
            _db = db;
        }

        [RelayCommand]
        private async Task PickImage()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a product image"
                });

                if (result is null) return;

                var localPath = Path.Combine(FileSystem.AppDataDirectory, $"product_{Guid.NewGuid()}.jpg");
                using var stream = await result.OpenReadAsync();
                using var fileStream = File.OpenWrite(localPath);
                await stream.CopyToAsync(fileStream);

                ImagePath = localPath;
                HasImage = true;
            }
            catch
            {
                ErrorMessage = "Could not load image. Try again.";
                HasError = true;
            }
        }

        [RelayCommand]
        private async Task CreateProduct()
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

            var newProduct = new ProductModel
            {
                Name = Name.Trim(),
                Description = Description.Trim(),
                Category = Category.Trim(),
                Price = parsedPrice,
                Quantity = parsedQty,
                ImagePath = ImagePath
            };

            await _db.AddProduct(newProduct);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
