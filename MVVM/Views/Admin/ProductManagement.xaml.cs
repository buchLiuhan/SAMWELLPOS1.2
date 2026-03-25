using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.Admin;

public partial class ProductManagement : ContentPage
{
    private readonly ProductManagementViewModel _vm;

    public ProductManagement(ProductManagementViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadProductsAsync();
    }
}