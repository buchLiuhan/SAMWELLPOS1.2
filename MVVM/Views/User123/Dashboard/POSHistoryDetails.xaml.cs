using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views.User123.Dashboard;

public partial class POSHistoryDetails : ContentPage
{
    public POSHistoryDetails(POSHistoryDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}