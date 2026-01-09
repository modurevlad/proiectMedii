using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileAPP.APIs;
using MobileAPP.Services;

namespace MobileAPP.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly APIAuth _authService;

    [ObservableProperty]
    private bool isAuthenticated;

    public MainPageViewModel(APIAuth authService)
    {
        _authService = authService;
        CheckAuthenticationStatus();
    }

    public void CheckAuthenticationStatus()
    {
        IsAuthenticated = AuthTokenService.HasToken();
    }

    [RelayCommand]
    private async Task NavigateToClientsAsync()
    {
        if (!await CheckAuthenticationAsync()) return;
        await Shell.Current.GoToAsync("//ClientsPage");
    }

    [RelayCommand]
    private async Task NavigateToCarsAsync()
    {
        if (!await CheckAuthenticationAsync()) return;
        await Shell.Current.GoToAsync("//CarsPage");
    }

    [RelayCommand]
    private async Task NavigateToServicesAsync()
    {
        if (!await CheckAuthenticationAsync()) return;
        await Shell.Current.GoToAsync("//ServicesPage");
    }

    [RelayCommand]
    private async Task NavigateToAppointmentsAsync()
    {
        if (!await CheckAuthenticationAsync()) return;
        await Shell.Current.GoToAsync("//AppointmentsPage");
    }

    [RelayCommand]
    private async Task NavigateToMechanicsAsync()
    {
        if (!await CheckAuthenticationAsync()) return;
        await Shell.Current.GoToAsync("//MechanicsPage");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _authService.Logout();
        AuthTokenService.ClearToken();
        IsAuthenticated = false;
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async Task<bool> CheckAuthenticationAsync()
    {
        if (!AuthTokenService.HasToken())
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return false;
        }
        return true;
    }
}