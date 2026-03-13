using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.Services;

namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty] private string? _email;
        [ObservableProperty] private string? _password;

        public LoginViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [RelayCommand]
        public async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Fill in your deets.", "My Bad");
                return;
            }

            var user = await _dbService.GetUserByEmail(Email);

            if (user != null && user.Password == Password)
            {
                // Safety check for non-admins
                if (!user.IsApproved && user.Role != "Admin")
                {
                    await Shell.Current.DisplayAlert("Hold Up", "Your account is pending approval.", "Copy");
                    return;
                }

                await Shell.Current.DisplayAlert("Welcome", $"Logged in as {user.Role}", "LFG");

                // ROLE-BASED ROUTING
                if (user.Role == "Admin")
                {
                    await Shell.Current.GoToAsync("UserManagement");
                }
                else
                {
                    await Shell.Current.GoToAsync("POSDashboard");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Denied", "Invalid credentials, G.", "Try Again");
            }
        }

        [RelayCommand]
        public async Task GoToRegister() => await Shell.Current.GoToAsync("Register");
    }
}
