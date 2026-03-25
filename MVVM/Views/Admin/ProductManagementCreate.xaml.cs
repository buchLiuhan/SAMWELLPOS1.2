using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.Admin;

public partial class ProductManagementCreate : ContentPage
{
    public ProductManagementCreate(ProductManagementCreateViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}