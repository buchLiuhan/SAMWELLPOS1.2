using SAMWELLPOS.MVVM.Views.Admin;

namespace SAMWELLPOS.MVVM.Views.Admin;

public partial class AdminShell : Shell
{
    public AdminShell()
    {
        InitializeComponent();

        // Register sub-page routes here (moved from AppShell)
        Routing.RegisterRoute("User_ManagementCreate", typeof(User_ManagementCreate));
        Routing.RegisterRoute("User_ManagementEdit", typeof(User_ManagementEdit));
        Routing.RegisterRoute("ProductManagementCreate", typeof(ProductManagementCreate));   // ← add
        Routing.RegisterRoute("ProductManagementEdit", typeof(ProductManagementEdit));       // ← add
        Routing.RegisterRoute("ProductManagementDetails", typeof(ProductManagementDetails)); // ← add
    }
}