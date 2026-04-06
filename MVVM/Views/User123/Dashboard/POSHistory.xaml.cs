using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.User123.Dashboard;

public partial class POSHistory : ContentPage
{
    private readonly POSHistoryViewModel _vm;

    public POSHistory(POSHistoryViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadHistoryAsync();
    }
}