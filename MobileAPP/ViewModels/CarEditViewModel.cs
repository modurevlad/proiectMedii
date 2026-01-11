using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileAPP.APIs;
using MobileAPP.Models;

namespace MobileAPP.ViewModels;

public partial class CarEditViewModel : ObservableObject
{
    private readonly APICar _carService;

    [ObservableProperty]
    private Car? car;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string brand = string.Empty;

    [ObservableProperty]
    private string model = string.Empty;

    [ObservableProperty]
    private int year = DateTime.Now.Year;

    [ObservableProperty]
    private string plateNumber = string.Empty;

    // Store the car ID separately to track edit mode more reliably
    private int _carIdForEdit = 0;

    public bool IsNotLoading => !IsLoading;
    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);
    
    [ObservableProperty]
    private bool isEditMode;

    public CarEditViewModel(APICar carService)
    {
        _carService = carService;
    }

    public void ResetViewModel()
    {
        Car = null;
        _carIdForEdit = 0;
        IsEditMode = false;
        Brand = string.Empty;
        Model = string.Empty;
        Year = DateTime.Now.Year;
        PlateNumber = string.Empty;
        ErrorMessage = string.Empty;
    }

    public async Task LoadCarAsync(int carId)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var loadedCar = await _carService.GetCarAsync(carId);
            
            if (loadedCar != null)
            {
                Car = loadedCar;
                _carIdForEdit = Car.Id;
                IsEditMode = _carIdForEdit > 0;
                Brand = Car.Brand;
                Model = Car.Model;
                Year = Car.Year;
                PlateNumber = Car.PlateNumber;
                Console.WriteLine($"[CarEditViewModel] Loaded car - ID={_carIdForEdit}, Brand={Brand}, Model={Model}, IsEditMode={IsEditMode}");
            }
            else
            {
                // Car not found - reset to create mode
                ResetViewModel();
                ErrorMessage = "Car not found. You can create a new car.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading car: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveCarAsync()
    {
        // Validation
        if (string.IsNullOrWhiteSpace(Brand))
        {
            ErrorMessage = "Brand is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Model))
        {
            ErrorMessage = "Model is required.";
            return;
        }

        if (Year < 1900 || Year > 2100)
        {
            ErrorMessage = "Year must be between 1900 and 2100.";
            return;
        }

        if (string.IsNullOrWhiteSpace(PlateNumber))
        {
            ErrorMessage = "Plate number is required.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            Car? savedCar;
            
            // PRIMARY CHECK: Use _carIdForEdit as the source of truth for edit mode
            // Fallback: If _carIdForEdit is not set but Car exists with an ID, use that
            var carIdToEdit = _carIdForEdit > 0 ? _carIdForEdit : (Car != null && Car.Id > 0 ? Car.Id : 0);
            var isEditing = carIdToEdit > 0;
            
            Console.WriteLine($"[CarEditViewModel] SaveCarAsync START - _carIdForEdit={_carIdForEdit}, carIdToEdit={carIdToEdit}, Car={(Car != null ? $"Id={Car.Id}" : "null")}, IsEditMode={IsEditMode}, isEditing={isEditing}, Brand={Brand}, Model={Model}");
            
            if (isEditing)
            {
                // We're in edit mode - ensure Car is loaded
                if (Car == null || Car.Id != carIdToEdit)
                {
                    Console.WriteLine($"[CarEditViewModel] Car is null or ID mismatch, loading car with ID={carIdToEdit}");
                    await LoadCarAsync(carIdToEdit);
                    if (Car == null)
                    {
                        ErrorMessage = "Car not found. Cannot update.";
                        IsLoading = false;
                        return;
                    }
                    // Update carIdToEdit after loading
                    carIdToEdit = Car.Id;
                }
                
                // Update existing car
                Car.Brand = Brand;
                Car.Model = Model;
                Car.Year = Year;
                Car.PlateNumber = PlateNumber;
                
                Console.WriteLine($"[CarEditViewModel] UPDATING car ID={carIdToEdit}, Brand={Brand}, Model={Model}");
                var success = await _carService.UpdateCarAsync(carIdToEdit, Car);
                savedCar = success ? Car : null;
            }
            else
            {
                // Create new car
                // ClientId will be set automatically by the backend based on the token
                Console.WriteLine($"[CarEditViewModel] CREATING new car - Brand={Brand}, Model={Model}");
                var newCar = new Car
                {
                    Brand = Brand,
                    Model = Model,
                    Year = Year,
                    PlateNumber = PlateNumber,
                    ClientId = 0 // Will be set by backend
                };
                savedCar = await _carService.CreateCarAsync(newCar);
            }

            if (savedCar != null)
            {
                var wasEditMode = isEditing;
                Car = savedCar;
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", 
                        wasEditMode ? "Car updated successfully." : "Car created successfully.", 
                        "OK");
                }
                
                // Navigate back to CarsPage
                await Shell.Current.GoToAsync("///CarsPage");
            }
            else
            {
                ErrorMessage = $"Failed to {(isEditing ? "update" : "create")} car. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving car: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("///CarsPage");
    }

    partial void OnIsLoadingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotLoading));
    }

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasErrorMessage));
    }
}
