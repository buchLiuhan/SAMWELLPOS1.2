using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.Admin;

public partial class User_Management : ContentPage
{
    private readonly UserManagementViewModel _vm;

    public User_Management(UserManagementViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadUsersAsync();
    }
}