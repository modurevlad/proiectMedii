using System.Security.Claims;
using MobileAPP.Models;

namespace MobileAPP.APIs;

public class APIClient
{
    private readonly HttpClient _httpClient;
    
    public APIClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}