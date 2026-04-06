using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;

namespace SAMWELLPOS.MVVM.ViewModels
{
    [QueryProperty(nameof(UserId), "UserId")]
    public partial class UserManagementEditViewModel : ObservableObject
    {
        private readonly DatabaseService _db;
        private readonly SessionService _session;
        private UserModel? _originalUser;

        [ObservableProperty]
        private int _userId;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _selectedRole = "Cashier";

        [ObservableProperty]
        private bool _isApproved = true;

        [ObservableProperty]
        private string? _profilePicturePath;

        [ObservableProperty]
        private bool _hasProfilePicture = false;

        [ObservableProperty]
        private string _initials = "?";

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError = false;

        [ObservableProperty]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        private string _confirmNewPassword = string.Empty;

        public List<string> Roles { get; } = new() { "Admin", "Cashier" };

        public UserManagementEditViewModel(DatabaseService db, SessionService session)
        {
            _db = db;
            _session = session;
        }

        partial void OnUserIdChanged(int value) => _ = LoadUserAsync(value);

        private async Task LoadUserAsync(int id)
        {
            _originalUser = await _db.GetUserById(id);
            if (_originalUser is null) return;

            FullName = _originalUser.FullName ?? string.Empty;
            Email = _originalUser.Email ?? string.Empty;
            SelectedRole = _originalUser.Role ?? "Cashier";
            IsApproved = _originalUser.IsApproved;
            ProfilePicturePath = _originalUser.ProfilePicturePath;
            HasProfilePicture = !string.IsNullOrEmpty(_originalUser.ProfilePicturePath);
            Initials = _originalUser.Initials;
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

        [RelayCommand]
        private async Task SaveChanges()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Full name and email cannot be empty.";
                HasError = true;
                return;
            }

            if (!string.IsNullOrWhiteSpace(NewPassword) || !string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                if (NewPassword.Length < 6)
                {
                    ErrorMessage = "New password must be at least 6 characters.";
                    HasError = true;
                    return;
                }

                if (NewPassword != ConfirmNewPassword)
                {
                    ErrorMessage = "Passwords do not match. Please check and try again.";
                    HasError = true;
                    return;
                }
            }

            if (_originalUser is null) return;

            _originalUser.FullName = FullName.Trim();
            _originalUser.Email = Email.Trim();
            _originalUser.Role = SelectedRole;
            _originalUser.IsApproved = IsApproved;
            _originalUser.ProfilePicturePath = ProfilePicturePath;

            if (!string.IsNullOrWhiteSpace(NewPassword))
                _originalUser.Password = NewPassword;

            await _db.UpdateUser(_originalUser);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task DeleteUser()
        {
            if (_session.IsCurrentUser(_originalUser!.Id))
            {
                await Shell.Current.DisplayAlert(
                    "Cannot Delete",
                    "You cannot delete your own account.",
                    "OK");
                return;
            }

            var allUsers = await _db.GetUsers();
            int adminCount = allUsers.Count(u => u.Role == "Admin");
            if (adminCount <= 1 && _originalUser.Role == "Admin")
            {
                await Shell.Current.DisplayAlert(
                    "Cannot Delete",
                    "This is the last admin account. The system must always have at least one admin.",
                    "OK");
                return;
            }

            bool confirmed = await Shell.Current.DisplayAlert(
                "Delete User",
                $"Are you sure you want to delete {FullName}? This cannot be undone.",
                "Delete", "Cancel");

            if (!confirmed) return;
            if (_originalUser is null) return;

            await _db.DeleteUser(_originalUser);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}