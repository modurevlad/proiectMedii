using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileAPP.APIs;
using MobileAPP.Models;

namespace MobileAPP.ViewModels;

public partial class ServicesViewModel : ObservableObject
{
    private readonly APIServiceCatalogItem _serviceService;

    [ObservableProperty]
    private ObservableCollection<ServiceCatalogItem> services = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private ServiceCatalogItem? selectedService;

    public bool IsEmpty => !IsLoading && Services.Count == 0;
    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ServicesViewModel(APIServiceCatalogItem serviceService)
    {
        _serviceService = serviceService;
    }

    [RelayCommand]
    private async Task LoadServicesAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var servicesList = await _serviceService.GetServiceCatalogItems();
            
            if (servicesList != null)
            {
                Services.Clear();
                foreach (var service in servicesList.OrderBy(s => s.Name))
                {
                    Services.Add(service);
                }
            }
            else
            {
                ErrorMessage = "Failed to load services. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading services: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(ServiceCatalogItem service)
    {
        if (service == null) return;
        
        SelectedService = service;
        // Show details in alert for now
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert("Service", 
                $"Service: {service.Name}\nPrice: {service.Price:C}", 
                "OK");
        }
    }

    partial void OnServicesChanged(ObservableCollection<ServiceCatalogItem> value)
    {
        OnPropertyChanged(nameof(IsEmpty));
    }

    partial void OnIsLoadingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsEmpty));
    }

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasErrorMessage));
    }
}