using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.User123.Dashboard;

public partial class POSDashboard : ContentPage
{
    private readonly POSDashboardViewModel _vm;

    public POSDashboard(POSDashboardViewModel vm)
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