using MobileAPP.ViewModels;

namespace MobileAPP.Pages;

public partial class CarEditPage : ContentPage
{
    private readonly CarEditViewModel _viewModel;

    public CarEditPage(CarEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Always reset first to clear any previous state
        _viewModel.ResetViewModel();
        
        // Wait a bit for Shell navigation to complete
        await Task.Delay(50);
        
        // Try to get carId from query parameters
        int? carId = null;
        
        if (Shell.Current?.CurrentState?.Location != null)
        {
            var uri = Shell.Current.CurrentState.Location.OriginalString;
            Console.WriteLine($"[CarEditPage] OnAppearing - Full URI: {uri}");
            
            var queryStart = uri.IndexOf('?');
            if (queryStart > 0 && queryStart < uri.Length - 1)
            {
                var query = uri.Substring(queryStart + 1);
                Console.WriteLine($"[CarEditPage] Query string: {query}");
                
                var parts = query.Split('&');
                foreach (var part in parts)
                {
                    var keyValue = part.Split('=');
                    if (keyValue.Length == 2 && keyValue[0].Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        var value = System.Uri.UnescapeDataString(keyValue[1]);
                        if (int.TryParse(value, out var parsedId) && parsedId > 0)
                        {
                            carId = parsedId;
                            Console.WriteLine($"[CarEditPage] Found carId: {carId}");
                            break;
                        }
                    }
                }
            }
        }
        
        // If we found a carId, load the car for editing
        if (carId.HasValue && carId.Value > 0)
        {
            Console.WriteLine($"[CarEditPage] Loading car ID: {carId.Value} for editing");
            await _viewModel.LoadCarAsync(carId.Value);
        }
        else
        {
            // No carId found, we're in create mode
            Console.WriteLine($"[CarEditPage] No carId found - CREATE MODE");
            _viewModel.ResetViewModel();
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///CarsPage");
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///CarsPage");
    }
}
