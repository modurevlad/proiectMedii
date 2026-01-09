using Microsoft.Extensions.Logging;
using MobileAPP.APIs;
using MobileAPP.Pages;
using MobileAPP.Services;
using MobileAPP.ViewModels;

namespace MobileAPP;

public static class MauiProgram
{
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
                BaseAddress = new Uri("https://localhost:7298"),
                Timeout = TimeSpan.FromSeconds(30)
            };
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
        builder.Services.AddTransient<ClientViewModel>();
        builder.Services.AddTransient<ServicesViewModel>();
        builder.Services.AddTransient<MechanicsViewModel>();

        // Register all Pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<AppointmentsPage>();
        builder.Services.AddTransient<CarsPage>();
        builder.Services.AddTransient<ClientsPage>();
        builder.Services.AddTransient<ServicesPage>();
        builder.Services.AddTransient<MechanicsPage>();

        return builder.Build();
    }
}