using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileAPP.APIs;
using MobileAPP.Models;

namespace MobileAPP.ViewModels;

public partial class MechanicsViewModel : ObservableObject
{
    private readonly APIMechanic _mechanicService;

    [ObservableProperty]
    private ObservableCollection<Mechanic> mechanics = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private Mechanic? selectedMechanic;

    public bool IsEmpty => !IsLoading && Mechanics.Count == 0;
    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    public MechanicsViewModel(APIMechanic mechanicService)
    {
        _mechanicService = mechanicService;
    }

    [RelayCommand]
    private async Task LoadMechanicsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var mechanicsList = await _mechanicService.GetMechanics();
            
            if (mechanicsList != null)
            {
                Mechanics.Clear();
                foreach (var mechanic in mechanicsList.OrderBy(m => m.Name))
                {
                    Mechanics.Add(mechanic);
                }
            }
            else
            {
                ErrorMessage = "Failed to load mechanics. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading mechanics: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(Mechanic mechanic)
    {
        if (mechanic == null) return;
        
        SelectedMechanic = mechanic;
        // Show details in alert for now
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert("Mechanic", 
                $"Name: {mechanic.Name}\nSpecialty: {mechanic.Specialty}", 
                "OK");
        }
    }

    partial void OnMechanicsChanged(ObservableCollection<Mechanic> value)
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