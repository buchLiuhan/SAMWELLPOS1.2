using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;

namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class UserManagementCreateViewModel : ObservableObject
    {
        private readonly DatabaseService _db;


        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _selectedRole = "Cashier";

        [ObservableProperty]
        private bool _isApproved = true;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        [ObservableProperty]
        private string? _profilePicturePath;

        [ObservableProperty]
        private bool _hasProfilePicture = false;

        public List<string> Roles { get; } = new() { "Admin", "Cashier" };

        public UserManagementCreateViewModel(DatabaseService db)
        {
            _db = db;
        }

        [RelayCommand]
        private async Task CreateUser()
        {
            ErrorMessage = string.Empty;
            HasError = false;

            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please fill in all fields.";
                HasError = true;
                return;
            }

            // Check duplicate username
            var existingUsername = await _db.GetUserByUsername(Username.Trim());
            if (existingUsername is not null)
            {
                ErrorMessage = "That username is already taken.";
                HasError = true;
                return;
            }

            // Check duplicate email
            var existingEmail = await _db.GetUserByEmail(Email.Trim());
            if (existingEmail is not null)
            {
                ErrorMessage = "An account with that email already exists.";
                HasError = true;
                return;
            }

            var newUser = new UserModel
            {
                FullName = FullName.Trim(),
                Username = Username.Trim(),
                Email = Email.Trim(),
                Password = Password,
                Role = SelectedRole,
                IsApproved = IsApproved,
                ProfilePicturePath = ProfilePicturePath
            };

            await _db.AddUser(newUser);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task PickPhoto()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a profile photo"
                });

                if (result is null) return;

                // Copy to app's local storage so the path persists
                var localPath = Path.Combine(FileSystem.AppDataDirectory, $"profile_{Guid.NewGuid()}.jpg");
                using var stream = await result.OpenReadAsync();
                using var fileStream = File.OpenWrite(localPath);
                await stream.CopyToAsync(fileStream);

                ProfilePicturePath = localPath;
                HasProfilePicture = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Could not load photo. Try again.";
                HasError = true;
            }
        }
    }
}