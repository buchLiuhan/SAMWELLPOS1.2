using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Views.Admin;
using SAMWELLPOS.Services;

namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty]
        private string? _username;

        [ObservableProperty]
        private string? _password;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        public LoginViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [RelayCommand]
        public async Task Login()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            // Field-specific validation
            if (string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your username and password.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Username is required.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required.";
                HasError = true;
                return;
            }

            var user = await _dbService.GetUserByUsername(Username.Trim());

            // User not found
            if (user is null)
            {
                ErrorMessage = "No account found with that username. Please check and try again.";
                HasError = true;
                return;
            }

            // Wrong password
            if (user.Password != Password)
            {
                ErrorMessage = "Incorrect password. Please try again.";
                HasError = true;
                return;
            }

            // Account not approved
            if (!user.IsApproved)
            {
                ErrorMessage = user.Role == "Admin"
                    ? "Your admin account is pending approval from an existing administrator."
                    : "Your account is pending approval from an administrator. Please wait.";
                HasError = true;
                return;
            }

            // Success — route by role
            HasError = false;
            if (user.Role == "Admin")
            {
                var adminShell = IPlatformApplication.Current!.Services
                    .GetRequiredService<AdminShell>();
                App.Current!.MainPage = adminShell;
            }
            else
            {
                await Shell.Current.GoToAsync("POSDashboard");
            }
        }

        [RelayCommand]
        public async Task GoToRegister() => await Shell.Current.GoToAsync("Register");
    }
}