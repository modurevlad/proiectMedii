using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileAPP.APIs;
using MobileAPP.Models;

namespace MobileAPP.ViewModels;

public partial class AppointmentsViewModel : ObservableObject
{
    private readonly APIAppointment _appointmentService;
    private readonly APICar _carService;
    private readonly APIMechanic _mechanicService;

    [ObservableProperty]
    private ObservableCollection<Appointment> appointments = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private Appointment? selectedAppointment;

    public bool IsEmpty => !IsLoading && Appointments.Count == 0;
    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    public AppointmentsViewModel(APIAppointment appointmentService, APICar carService, APIMechanic mechanicService)
    {
        _appointmentService = appointmentService;
        _carService = carService;
        _mechanicService = mechanicService;
    }

    [RelayCommand]
    private async Task LoadAppointmentsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var appointmentsList = await _appointmentService.GetAppointmentsAsync();
            
            if (appointmentsList != null)
            {
                Appointments.Clear();
                foreach (var appointment in appointmentsList.OrderByDescending(a => a.AppointmentDate))
                {
                    Appointments.Add(appointment);
                }
            }
            else
            {
                ErrorMessage = "Failed to load appointments. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading appointments: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAppointmentAsync(Appointment appointment)
    {
        if (appointment == null) return;

        bool confirm = false;
        if (Application.Current?.MainPage != null)
        {
            confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Appointment",
                $"Are you sure you want to delete the appointment on {appointment.AppointmentDate:MM/dd/yyyy}?",
                "Delete",
                "Cancel");
        }

        if (!confirm) return;

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var success = await _appointmentService.DeleteAppointmentAsync(appointment.Id);
            
            if (success)
            {
                Appointments.Remove(appointment);
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Appointment deleted successfully.", "OK");
                }
            }
            else
            {
                ErrorMessage = "Failed to delete appointment. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error deleting appointment: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(Appointment appointment)
    {
        if (appointment == null) return;
        
        SelectedAppointment = appointment;
        // TODO: Navigate to detail page when created
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert("Appointment", 
                $"Appointment ID: {appointment.Id}\nDate: {appointment.AppointmentDate:MM/dd/yyyy HH:mm}\nStatus: {appointment.Status}", 
                "OK");
        }
    }

    [RelayCommand]
    private async Task NavigateToNewAsync()
    {
        // TODO: Navigate to new appointment page when created
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert("New Appointment", "New appointment page coming soon!", "OK");
        }
    }

    partial void OnAppointmentsChanged(ObservableCollection<Appointment> value)
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
