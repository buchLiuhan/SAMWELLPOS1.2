using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.Admin;

public partial class ProductManagementEdit : ContentPage
{
    public ProductManagementEdit(ProductManagementEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}