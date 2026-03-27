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
            Routing.RegisterRoute("User_ManagementCreate", typeof(SAMWELLPOS.MVVM.Views.Admin.User_ManagementCreate)); // ← add this
            Routing.RegisterRoute("User_ManagementEdit", typeof(SAMWELLPOS.MVVM.Views.Admin.User_ManagementEdit));

            // Cashier/POS side
            Routing.RegisterRoute("POSDashboard", typeof(SAMWELLPOS.MVVM.Views.User123.Dashboard.POSDashboard));
            Routing.RegisterRoute("POSCart", typeof(SAMWELLPOS.MVVM.Views.User123.Dashboard.POSCart));
            Routing.RegisterRoute("POSPayment", typeof(SAMWELLPOS.MVVM.Views.User123.Dashboard.POSPayment));
            Routing.RegisterRoute("POSReceipt", typeof(SAMWELLPOS.MVVM.Views.User123.Dashboard.POSReceipt));
        }
    }
}
