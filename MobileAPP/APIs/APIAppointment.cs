using System.Net;
using System.Net.Http.Json;
using MobileAPP.Models;

namespace MobileAPP.APIs;

public class APIAppointment
{
    private readonly HttpClient _httpClient;

    public APIAppointment(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Appointment>?> GetAppointmentsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Appointment>>("api/Appointments");
        }
        catch
        {
            return null;
        }
    }

    public async Task<Appointment?> GetAppointmentAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Appointment>($"api/Appointments/{id}");
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

    public async Task<Appointment?> CreateAppointmentAsync(Appointment appointment)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Appointments", appointment);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Appointment>();
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAppointmentAsync(int id, Appointment appointment)
    {
        try
        {
            appointment.Id = id;
            var response = await _httpClient.PutAsJsonAsync($"api/Appointments/{id}", appointment);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Appointments/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
