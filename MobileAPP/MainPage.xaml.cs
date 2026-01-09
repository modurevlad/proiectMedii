using MobileAPP.ViewModels;

namespace MobileAPP;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CheckAuthenticationStatus();
    }
}