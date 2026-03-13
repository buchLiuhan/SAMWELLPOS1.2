
using SAMWELLPOS.MVVM.ViewModels;

namespace SAMWELLPOS.MVVM.Views;

public partial class Login : ContentPage
{
	public Login(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}