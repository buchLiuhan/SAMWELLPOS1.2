using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.Admin;

public partial class User_ManagementCreate : ContentPage
{
    public User_ManagementCreate(UserManagementCreateViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}