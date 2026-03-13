using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views;

public partial class Register : ContentPage
{
	public Register(RegistrationViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}