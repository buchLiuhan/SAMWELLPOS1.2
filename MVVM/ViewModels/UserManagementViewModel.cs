using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;
using System.Collections.ObjectModel;
using SAMWELLPOS.MVVM.Views;


namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        private readonly DatabaseService _db;
        private List<UserModel> _allUsers = new();

        [ObservableProperty]
        private ObservableCollection<UserModel> _filteredUsers = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        public UserManagementViewModel(DatabaseService db)
        {
            _db = db;
        }

        public async Task LoadUsersAsync()
        {
            _allUsers = await _db.GetUsers();
            ApplyFilter();
        }

        partial void OnSearchTextChanged(string value) => ApplyFilter();

        private void ApplyFilter()
        {
            var query = string.IsNullOrWhiteSpace(SearchText)
                ? _allUsers
                : _allUsers.Where(u =>
                    (u.FullName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.Role?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));

            FilteredUsers = new ObservableCollection<UserModel>(query);
        }

        [RelayCommand]
        private async Task NavigateToCreate()
        {
            await Shell.Current.GoToAsync("User_ManagementCreate");
        }
        [RelayCommand]
        private async Task NavigateToEdit(int userId)
        {
            await Shell.Current.GoToAsync($"User_ManagementEdit?UserId={userId}");
        }

        [RelayCommand]
        private async Task Logout()
        {
            bool confirmed = await Shell.Current.DisplayAlert(
                "Logout",
                "Are you sure you want to log out?",
                "Logout",
                "Cancel");

            if (!confirmed) return;

            // Navigate back to login and clear the back stack
            var appShell = IPlatformApplication.Current!.Services.GetRequiredService<AppShell>();
            App.Current!.MainPage = appShell;
            await Shell.Current.GoToAsync("//Login");
        }
    }
}