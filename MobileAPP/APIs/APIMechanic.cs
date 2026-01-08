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
    
    public async Task<IEnumerable<Mechanic>> GetMechanics()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Mechanic>>("api/Mechanics");
    }

    public async Task<Mechanic> GetMechanic(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Mechanic>($"api/Mechanics/{id}");
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}