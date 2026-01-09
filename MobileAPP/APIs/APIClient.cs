using System.Net;
using System.Net.Http.Json;
using MobileAPP.Models;

namespace MobileAPP.APIs;

public class APIClient
{
    private readonly HttpClient _httpClient;
    
    public APIClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Client?> GetCurrentClientAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Client>("api/Clients");
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

    public async Task<Client?> GetClientAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Client>($"api/Clients/{id}");
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

    public async Task<Client?> CreateClientAsync(Client client)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Clients", client);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Client>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateClientAsync(int id, Client client)
    {
        try
        {
            client.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"api/Clients/{id}", client);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
