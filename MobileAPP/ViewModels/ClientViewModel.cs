using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileAPP.APIs;
using MobileAPP.Models;

namespace MobileAPP.ViewModels;

public partial class ClientViewModel : ObservableObject
{
    private readonly APIClient _clientService;

    [ObservableProperty]
    private Client? client;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string phone = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private bool isEditing;

    public bool IsNotEditing => !IsEditing;
    public bool IsNotLoading => !IsLoading;
    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ClientViewModel(APIClient clientService)
    {
        _clientService = clientService;
    }

    [RelayCommand]
    private async Task LoadClientAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var loadedClient = await _clientService.GetCurrentClientAsync();
            
            if (loadedClient != null)
            {
                Client = loadedClient;
                Name = Client.Name;
                Phone = Client.Phone;
                Email = Client.Email;
            }
            else
            {
                ErrorMessage = "No profile found. Please create your profile.";
                IsEditing = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading profile: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveClientAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "All fields are required.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            Client? savedClient;
            
            if (Client == null)
            {
                // Create new client
                var newClient = new Client
                {
                    Name = Name,
                    Phone = Phone,
                    Email = Email
                };
                savedClient = await _clientService.CreateClientAsync(newClient);
            }
            else
            {
                // Update existing client
                Client.Name = Name;
                Client.Phone = Phone;
                Client.Email = Email;
                var success = await _clientService.UpdateClientAsync(Client.Id, Client);
                savedClient = success ? Client : null;
            }

            if (savedClient != null)
            {
                Client = savedClient;
                IsEditing = false;
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Profile saved successfully.", "OK");
                }
            }
            else
            {
                ErrorMessage = "Failed to save profile. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving profile: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void StartEditing()
    {
        IsEditing = true;
    }

    [RelayCommand]
    private void CancelEditing()
    {
        if (Client != null)
        {
            Name = Client.Name;
            Phone = Client.Phone;
            Email = Client.Email;
        }
        IsEditing = false;
    }

    partial void OnIsEditingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotEditing));
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
