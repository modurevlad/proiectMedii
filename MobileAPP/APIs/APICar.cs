using System.Net;
using System.Net.Http.Json;
using MobileAPP.Models;

namespace MobileAPP.APIs;

public class APICar
{
    private readonly HttpClient _httpClient;
    
    public APICar(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Car>?> GetCarsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Car>>("api/Cars");
        }
        catch
        {
            return null;
        }
    }

    public async Task<Car?> GetCarAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Car>($"api/Cars/{id}");
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

    public async Task<Car?> CreateCarAsync(Car car)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Cars", car);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Car>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateCarAsync(int id, Car car)
    {
        try
        {
            car.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"api/Cars/{id}", car);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteCarAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Cars/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
