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
        catch
        {
            return null;
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
        catch
        {
            return null;
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
