using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;
using System.Collections.ObjectModel;

namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        private readonly DatabaseService _db;
        private readonly SessionService _session;
        private List<UserModel> _allUsers = new();

        [ObservableProperty]
        private ObservableCollection<UserModel> _filteredUsers = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        public UserManagementViewModel(DatabaseService db, SessionService session)
        {
            _db = db;
            _session = session;
        }

        public async Task LoadUsersAsync()
        {
            _allUsers = await _db.GetUsers();
            ApplyFilter();
        }

        partial void OnSearchTextChanged(string value) => ApplyFilter();

        private void ApplyFilter()
        {
            var currentId = _session.CurrentUser?.Id ?? -1;

            // Show: current user + cashiers + unapproved
            // Hide: other admins
            var query = _allUsers.Where(u =>
                u.Id == currentId ||           // always show self
                u.Role != "Admin"              // show cashiers + unapproved
            );

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(u =>
                    (u.FullName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.Role?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            // Stamp IsCurrentUser on each
            var list = query.ToList();
            foreach (var u in list)
                u.IsCurrentUser = u.Id == currentId;

            // Sort: current user always first (top-left)
            var sorted = list
                .OrderByDescending(u => u.IsCurrentUser)
                .ThenBy(u => u.FullName)
                .ToList();

            FilteredUsers = new ObservableCollection<UserModel>(sorted);
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

            _session.Clear();
            App.Current!.MainPage = new AppShell();
        }
    }
}