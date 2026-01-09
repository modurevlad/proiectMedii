using MobileAPP.Services;

namespace MobileAPP;

public partial class AppShell : Shell
{
    private bool _initialNavigationHandled = false;

    public AppShell()
    {
        InitializeComponent();
        
        // Handle initial navigation based on authentication
        Navigated += OnShellNavigated;
        
        // Also check immediately after initialization
        Task.Run(async () =>
        {
            await Task.Delay(100); // Small delay to ensure Shell is fully initialized
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!_initialNavigationHandled)
                {
                    _initialNavigationHandled = true;
                    
                    if (AuthTokenService.HasToken())
                    {
                        await GoToAsync("//MainPage");
                    }
                    else
                    {
                        await GoToAsync("//LoginPage");
                    }
                }
            });
        });
    }

    private async void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        // Handle initial navigation only once
        if (!_initialNavigationHandled && e.Source == ShellNavigationSource.ShellItemChanged)
        {
            _initialNavigationHandled = true;
            
            // Check authentication and route accordingly
            if (AuthTokenService.HasToken())
            {
                await GoToAsync("//MainPage");
            }
            else
            {
                await GoToAsync("//LoginPage");
            }
            return;
        }
        
        // Check authentication on navigation to protected routes
        var currentRoute = CurrentState?.Location?.OriginalString ?? string.Empty;
        
        // If navigating to a protected route and not authenticated, redirect to login
        if (!string.IsNullOrEmpty(currentRoute) &&
            !currentRoute.Contains("LoginPage") && 
            !currentRoute.Contains("MainPage") &&
            !AuthTokenService.HasToken())
        {
            await GoToAsync("//LoginPage");
        }
    }
}