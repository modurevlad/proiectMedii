using Microsoft.Extensions.Logging;
using MobileAPP.APIs;
using MobileAPP.Pages;
using MobileAPP.Services;
using MobileAPP.ViewModels;

namespace MobileAPP;

public static class MauiProgram
{
    // API Configuration - Easy to change base URL here
    // If localhost doesn't work, try:
    // - "https://127.0.0.1:7298" (for iOS Simulator if localhost fails)
    // - "http://localhost:5050" (if using HTTP instead of HTTPS)
    // - "https://YOUR_MAC_IP:7298" (for physical iOS device, replace YOUR_MAC_IP with your Mac's IP)
    private const string API_BASE_URL = "https://localhost:7298";
    private const int API_TIMEOUT_SECONDS = 30;
    
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Configure HttpClient with base address and authentication handler
        builder.Services.AddSingleton<HttpClient>(sp =>
        {
            HttpClientHandler handler;
            
#if DEBUG
            // Bypass SSL certificate validation for localhost in DEBUG mode
            handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
#else
            // Use default validation in RELEASE mode
            handler = new HttpClientHandler();
#endif
            
            var authHandler = new AuthHeaderHandler
            {
                InnerHandler = handler
            };
            
            var httpClient = new HttpClient(authHandler)
            {
                BaseAddress = new Uri(API_BASE_URL),
                Timeout = TimeSpan.FromSeconds(API_TIMEOUT_SECONDS)
            };
            
            System.Diagnostics.Debug.WriteLine($"HttpClient configured with BaseAddress: {API_BASE_URL}");
            return httpClient;
        });

        // Register all API services
        builder.Services.AddSingleton<APIAuth>();
        builder.Services.AddSingleton<APIAppointment>();
        builder.Services.AddSingleton<APIAppointementService>();
        builder.Services.AddSingleton<APICar>();
        builder.Services.AddSingleton<APIClient>();
        builder.Services.AddSingleton<APIMechanic>();
        builder.Services.AddSingleton<APIServiceCatalogItem>();

        // Register all ViewModels
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<AppointmentsViewModel>();
        builder.Services.AddTransient<CarsViewModel>();
        builder.Services.AddTransient<CarEditViewModel>();
        builder.Services.AddTransient<ClientViewModel>();
        builder.Services.AddTransient<ServicesViewModel>();
        builder.Services.AddTransient<MechanicsViewModel>();

        // Register all Pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<AppointmentsPage>();
        builder.Services.AddTransient<CarsPage>();
        builder.Services.AddTransient<CarEditPage>();
        builder.Services.AddTransient<ClientsPage>();
        builder.Services.AddTransient<ServicesPage>();
        builder.Services.AddTransient<MechanicsPage>();

        return builder.Build();
    }
}