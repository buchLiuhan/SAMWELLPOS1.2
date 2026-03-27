using SAMWELLPOS.MVVM.ViewModels;
using SAMWELLPOS.Services;

namespace SAMWELLPOS.MVVM.Views.User123.Dashboard;

public partial class POSReceipt : ContentPage
{
    private readonly POSReceiptViewModel _vm;
    private readonly CartService _cart;

    public POSReceipt(POSReceiptViewModel vm, CartService cart)
    {
        InitializeComponent();
        _vm = vm;
        _cart = cart;
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadReceipt(_cart.Items);
    }
}