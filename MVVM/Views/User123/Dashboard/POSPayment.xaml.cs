using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.User123.Dashboard;

public partial class POSPayment : ContentPage
{
    private readonly POSPaymentViewModel _vm;

    public POSPayment(POSPaymentViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadTotal();
    }
}