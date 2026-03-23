using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.Admin;

public partial class User_ManagementEdit : ContentPage
{
    public User_ManagementEdit(UserManagementEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}