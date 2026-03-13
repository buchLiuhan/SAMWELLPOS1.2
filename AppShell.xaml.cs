namespace SAMWELLPOS
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("Register", typeof(SAMWELLPOS.MVVM.Views.Register));

            // Admin side
            Routing.RegisterRoute("UserManagement", typeof(SAMWELLPOS.MVVM.Views.Admin.User_Management));

            // Cashier/POS side
            Routing.RegisterRoute("POSDashboard", typeof(SAMWELLPOS.MVVM.Views.User123.Dashboard.POSDashboard));
        }
    }
}
