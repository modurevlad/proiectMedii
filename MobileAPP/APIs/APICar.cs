using System.Security.Claims;
using MobileAPP.Models;

namespace MobileAPP.APIs;

public class APICar
{
    private readonly HttpClient _httpClient;
    
    public APICar(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}