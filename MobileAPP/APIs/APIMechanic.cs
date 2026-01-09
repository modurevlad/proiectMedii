using System.Net;
using System.Net.Http.Json;
using MobileAPP.Models;

namespace MobileAPP.APIs;

public class APIMechanic
{
    private readonly HttpClient _httpClient;
    
    public APIMechanic(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IEnumerable<Mechanic>?> GetMechanics()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Mechanic>>("api/Mechanics");
        }
        catch
        {
            return null;
        }
    }

    public async Task<Mechanic?> GetMechanic(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Mechanic>($"api/Mechanics/{id}");
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }
}