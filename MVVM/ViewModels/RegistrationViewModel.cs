using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;

namespace SAMWELLPOS.MVVM.ViewModels
{
    // "partial" is required for the CommunityToolkit magic to work
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty] private string? _fullName;
        [ObservableProperty] private string? _email;
        [ObservableProperty] private string? _password;
        [ObservableProperty] private string? _confirmPassword;

        public RegistrationViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [RelayCommand]
        public async Task Register()
        {
            // 1. Thorough Validation Logic
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Shell.Current.DisplayAlert("Wait!", "You gotta fill everything out, G.", "My Bad");
                return;
            }

            // 2. Password Match Check (The UX Safety Net)
            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Passwords don't match! Check your typing.", "Try Again");
                return;
            }

            // 3. KING MOVE: Check if this is the first user in the system
            var users = await _dbService.GetUsers();
            bool isFirstUser = users.Count == 0;

            // 4. Map the data to our Model
            var newUser = new UserModel
            {
                FullName = FullName,
                Email = Email ?? string.Empty,
                Password = Password,
                Role = isFirstUser ? "Admin" : "Cashier", // First user gets the crown
                IsApproved = isFirstUser                  // First user is auto-vouched
            };

            // 5. Save to SQLite
            await _dbService.AddUser(newUser);

            string alertMsg = isFirstUser
                ? "First user detected! Admin access granted. You're the boss now."
                : "Account created! Now wait for the Admin to approve you.";

            await Shell.Current.DisplayAlert("LFG!", alertMsg, "Bet");

            // 6. Navigate back to Login
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task GoToLogin()
        {
            // Quickly pop back to the Login screen
            await Shell.Current.GoToAsync("..");
        }
    }
}