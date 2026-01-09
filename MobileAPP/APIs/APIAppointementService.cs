using System.Net;
using System.Net.Http.Json;
using MobileAPP.Models;

namespace MobileAPP.APIs;

public class APIAppointementService
{
    private readonly HttpClient _httpClient;

    public APIAppointementService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<AppointmentService>?> GetAppointmentServicesAsync(int? appointmentId = null)
    {
        try
        {
            var url = "api/AppointmentServices";
            if (appointmentId.HasValue)
            {
                url += $"?appointmentId={appointmentId.Value}";
            }
            return await _httpClient.GetFromJsonAsync<IEnumerable<AppointmentService>>(url);
        }
        catch
        {
            return null;
        }
    }

    public async Task<AppointmentService?> GetAppointmentServiceAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AppointmentService>($"api/AppointmentServices/{id}");
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

    public async Task<AppointmentService?> CreateAppointmentServiceAsync(AppointmentService appointmentService)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/AppointmentServices", appointmentService);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AppointmentService>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAppointmentServiceAsync(int id, AppointmentService appointmentService)
    {
        try
        {
            appointmentService.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"api/AppointmentServices/{id}", appointmentService);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAppointmentServiceAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/AppointmentServices/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
