using Microsoft.Extensions.Logging;
using SAMWELLPOS.Services;
using SAMWELLPOS.MVVM.ViewModels;
using SAMWELLPOS.MVVM.Views;
using SAMWELLPOS.MVVM.Views.Admin;
using CommunityToolkit.Maui;

namespace SAMWELLPOS
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
    // Kills the Android underline
    handler.PlatformView.Background = null;
    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif WINDOWS
    // Kills the Windows border
    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
            });

            // 1. Register the Database Service as a Singleton
            builder.Services.AddSingleton<DatabaseService>();

            // 2. Register your ViewModels (We'll build these next!)
            // Transient means a fresh copy is created every time you go to the page
            builder.Services.AddTransient<RegistrationViewModel>();

            // 3. Register your Views
            builder.Services.AddTransient<Register>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<Login>(); // Or whatever your login view is named

            builder.Services.AddTransient<UserManagementViewModel>();
            builder.Services.AddTransient<User_Management>();

            builder.Services.AddTransient<UserManagementCreateViewModel>();
            builder.Services.AddTransient<User_ManagementCreate>();

            return builder.Build();


        }
    }
}
