using Microsoft.Maui.Storage;

namespace MobileAPP.Services;

public class AuthTokenService
{
    private const string TokenKey = "auth_token";
    private static string? _currentToken;

    public static void SetToken(string? token)
    {
        _currentToken = token;
        if (string.IsNullOrEmpty(token))
        {
            Preferences.Remove(TokenKey);
        }
        else
        {
            Preferences.Set(TokenKey, token);
        }
    }

    public static string? GetToken()
    {
        if (_currentToken != null)
        {
            return _currentToken;
        }
        
        _currentToken = Preferences.Get(TokenKey, null);
        return _currentToken;
    }

    public static void ClearToken()
    {
        _currentToken = null;
        Preferences.Remove(TokenKey);
    }

    public static bool HasToken()
    {
        return !string.IsNullOrEmpty(GetToken());
    }
}
