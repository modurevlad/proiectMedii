using System.Net.Http.Json;
using System.Text.Json.Serialization;
using MobileAPP.Services;

namespace MobileAPP.APIs;

public class APIAuth
{
    private readonly HttpClient _httpClient;

    public APIAuth(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", request);
            
            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse?.Token != null)
                {
                    AuthTokenService.SetToken(loginResponse.Token);
                }
                return loginResponse;
            }
            
            return null;
        }
        catch (HttpRequestException ex)
        {
            // Provide more specific error information
            System.Diagnostics.Debug.WriteLine($"HTTP Error during registration: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            throw new Exception($"Cannot connect to server. Please ensure the server is running at {_httpClient.BaseAddress}. Error: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new Exception("Connection timeout. The server may not be running or is not accessible.", ex);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Registration Error: {ex.GetType().Name}: {ex.Message}");
            throw;
        }
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse?.Token != null)
                {
                    AuthTokenService.SetToken(loginResponse.Token);
                }
                return loginResponse;
            }
            
            return null;
        }
        catch (HttpRequestException ex)
        {
            // Provide more specific error information
            System.Diagnostics.Debug.WriteLine($"HTTP Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            throw new Exception($"Cannot connect to server. Please ensure the server is running at {_httpClient.BaseAddress}. Error: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new Exception("Connection timeout. The server may not be running or is not accessible.", ex);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login Error: {ex.GetType().Name}: {ex.Message}");
            throw;
        }
    }

    public void Logout()
    {
        AuthTokenService.ClearToken();
    }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
