using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;
using System.Text.RegularExpressions;

namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty]
        private string? _fullName;

        [ObservableProperty]
        private string? _username;

        [ObservableProperty]
        private string? _email;

        [ObservableProperty]
        private string? _password;

        [ObservableProperty]
        private string? _confirmPassword;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;


        [ObservableProperty]
        private string? _profilePicturePath;

        [ObservableProperty]
        private bool _hasProfilePicture = false;


        public RegistrationViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [RelayCommand]
        public async Task Register()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            // Field-specific validation
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Full name is required.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Username is required.";
                HasError = true;
                return;
            }

            if (Username!.Trim().Length < 3)
            {
                ErrorMessage = "Username must be at least 3 characters.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email address is required.";
                HasError = true;
                return;
            }

            if (!Regex.IsMatch(Email!, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ErrorMessage = "Please enter a valid email address.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required.";
                HasError = true;
                return;
            }

            if (Password!.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Please confirm your password.";
                HasError = true;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match. Please check and try again.";
                HasError = true;
                return;
            }

            // Duplicate username check
            var existingUsername = await _dbService.GetUserByUsername(Username.Trim());
            if (existingUsername is not null)
            {
                ErrorMessage = "That username is already taken. Please choose another.";
                HasError = true;
                return;
            }

            // Duplicate email check
            var existingEmail = await _dbService.GetUserByEmail(Email.Trim());
            if (existingEmail is not null)
            {
                ErrorMessage = "An account with that email already exists.";
                HasError = true;
                return;
            }

            // First user gets Admin + auto-approved
            var users = await _dbService.GetUsers();
            bool isFirstUser = users.Count == 0;

            var newUser = new UserModel
            {
                FullName = FullName!.Trim(),
                Username = Username.Trim(),
                Email = Email.Trim(),
                Password = Password,
                Role = isFirstUser ? "Admin" : "Cashier",
                IsApproved = isFirstUser,
                ProfilePicturePath = ProfilePicturePath  // ← add this
            };

            await _dbService.AddUser(newUser);

            string successMsg = isFirstUser
                ? "You are the first user — Admin access has been granted and your account is approved automatically."
                : "Account created successfully. Please wait for an administrator to approve your account before logging in.";

            await Shell.Current.DisplayAlert("Registration Successful", successMsg, "OK");
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task GoToLogin() => await Shell.Current.GoToAsync("..");

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

                var localPath = Path.Combine(FileSystem.AppDataDirectory, $"profile_{Guid.NewGuid()}.jpg");
                using var stream = await result.OpenReadAsync();
                using var fileStream = File.OpenWrite(localPath);
                await stream.CopyToAsync(fileStream);

                ProfilePicturePath = localPath;
                HasProfilePicture = true;
            }
            catch
            {
                ErrorMessage = "Could not load photo. Try again.";
                HasError = true;
            }
        }
    }
}