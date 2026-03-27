using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.User123.Dashboard;

public partial class POSCart : ContentPage
{
    private readonly POSCartViewModel _vm;

    public POSCart(POSCartViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.RefreshCart();
    }
}