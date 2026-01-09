using MobileAPP.Services;
using MobileAPP.ViewModels;

namespace MobileAPP.Pages;

public partial class MechanicsPage : ContentPage
{
    public MechanicsPage(MechanicsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (!await CheckAuthenticationAsync()) return;
        
        if (BindingContext is MechanicsViewModel vm)
        {
            await vm.LoadMechanicsCommand.ExecuteAsync(null);
        }
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

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}