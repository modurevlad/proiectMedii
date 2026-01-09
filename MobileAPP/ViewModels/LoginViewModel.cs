using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileAPP.APIs;

namespace MobileAPP.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly APIAuth _authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isRegisterMode;

    public string TitleText => IsRegisterMode ? "Register" : "Login";
    public bool ShowConfirmPassword => IsRegisterMode;
    public bool ShowLoginButton => !IsRegisterMode;
    public bool ShowRegisterButton => IsRegisterMode;
    public string ToggleButtonText => IsRegisterMode ? "Already have an account? Login" : "Don't have an account? Register";
    public bool IsPasswordValid => string.IsNullOrEmpty(Password) || Password.Length >= 6;
    public bool IsNotLoading => !IsLoading;
    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    public LoginViewModel(APIAuth authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email and password are required.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var loginRequest = new LoginRequest
            {
                Email = Email,
                Password = Password
            };

            var response = await _authService.LoginAsync(loginRequest);

            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                ErrorMessage = "Invalid email or password. Please try again.";
            }
        }
        catch (Exception ex)
        {
            // Check if it's a connection error
            if (ex.Message.Contains("connect", StringComparison.OrdinalIgnoreCase) || 
                ex.Message.Contains("refused", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
            {
                ErrorMessage = $"Cannot connect to server.\n\nPlease verify:\n1. The server is running\n2. The server is accessible\n3. Check your network connection\n\nError: {ex.Message}";
            }
            else
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
            
            System.Diagnostics.Debug.WriteLine($"Login Error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email and password are required.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters long.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var registerRequest = new RegisterRequest
            {
                Email = Email,
                Password = Password
            };

            var response = await _authService.RegisterAsync(registerRequest);

            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                // Registration successful, automatically logged in
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                ErrorMessage = "Registration failed. The email may already be in use.";
            }
        }
        catch (Exception ex)
        {
            // Check if it's a connection error
            if (ex.Message.Contains("connect", StringComparison.OrdinalIgnoreCase) || 
                ex.Message.Contains("refused", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
            {
                ErrorMessage = $"Cannot connect to server.\n\nPlease verify:\n1. The server is running\n2. The server is accessible\n3. Check your network connection\n\nError: {ex.Message}";
            }
            else
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
            }
            
            System.Diagnostics.Debug.WriteLine($"Registration Error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ToggleMode()
    {
        IsRegisterMode = !IsRegisterMode;
        ErrorMessage = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
    }

    partial void OnIsRegisterModeChanged(bool value)
    {
        OnPropertyChanged(nameof(TitleText));
        OnPropertyChanged(nameof(ShowConfirmPassword));
        OnPropertyChanged(nameof(ShowLoginButton));
        OnPropertyChanged(nameof(ShowRegisterButton));
        OnPropertyChanged(nameof(ToggleButtonText));
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
