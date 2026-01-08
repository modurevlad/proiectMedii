using System.Net;
using System.Net.Http.Json;
using MobileAPP.Models;

namespace MobileAPP.APIs;

public class APIServiceCatalogItem
{
    private readonly HttpClient _httpClient;

    public APIServiceCatalogItem(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ServiceCatalogItem>> GetServiceCatalogItems()
    {
       return await _httpClient.GetFromJsonAsync<IEnumerable<ServiceCatalogItem>>("api/ServiceCatalogItems");
    }
    
    public async Task<ServiceCatalogItem> GetServiceCatalogItem(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ServiceCatalogItem>($"api/ServiceCatalogItems/{id}");
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        
    }
    
    
}