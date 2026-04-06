using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.Admin;

public partial class AdminHistory : ContentPage
{
    private readonly AdminHistoryViewModel _vm;

    public AdminHistory(AdminHistoryViewModel vm)
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