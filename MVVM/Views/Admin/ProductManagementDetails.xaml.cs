using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.Admin;

public partial class ProductManagementDetails : ContentPage
{
    public ProductManagementDetails(ProductManagementDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}