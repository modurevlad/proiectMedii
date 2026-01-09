using MobileAPP.Services;

namespace MobileAPP;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
        
        // Set initial route based on authentication
        // This will be handled in AppShell's navigation event
    }
}