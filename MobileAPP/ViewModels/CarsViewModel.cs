using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileAPP.APIs;
using MobileAPP.Models;

namespace MobileAPP.ViewModels;

public partial class CarsViewModel : ObservableObject
{
    private readonly APICar _carService;

    [ObservableProperty]
    private ObservableCollection<Car> cars = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool IsEmpty => !IsLoading && Cars.Count == 0;
    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    public CarsViewModel(APICar carService)
    {
        _carService = carService;
    }

    [RelayCommand]
    private async Task LoadCarsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var carsList = await _carService.GetCarsAsync();
            
            if (carsList != null)
            {
                Cars.Clear();
                foreach (var car in carsList)
                {
                    Cars.Add(car);
                }
            }
            else
            {
                ErrorMessage = "Failed to load cars. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading cars: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteCarAsync(Car car)
    {
        if (car == null) return;

        bool confirm = false;
        if (Application.Current?.MainPage != null)
        {
            confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Car",
                $"Are you sure you want to delete {car.Brand} {car.Model}?",
                "Delete",
                "Cancel");
        }

        if (!confirm) return;

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var success = await _carService.DeleteCarAsync(car.Id);
            
            if (success)
            {
                Cars.Remove(car);
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Car deleted successfully.", "OK");
                }
            }
            else
            {
                ErrorMessage = "Failed to delete car. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error deleting car: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(Car car)
    {
        if (car == null) return;
        
        // Navigate to edit page with car ID
        Console.WriteLine($"[CarsViewModel] Navigating to edit car ID={car.Id}");
        await Shell.Current.GoToAsync($"///CarEditPage?id={car.Id}");
    }

    [RelayCommand]
    private async Task NavigateToNewAsync()
    {
        // Navigate to create page
        await Shell.Current.GoToAsync("///CarEditPage");
    }

    partial void OnCarsChanged(ObservableCollection<Car> value)
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
