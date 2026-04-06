using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.Admin;

public partial class AdminHistoryDetails : ContentPage
{
    public AdminHistoryDetails(AdminHistoryDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}