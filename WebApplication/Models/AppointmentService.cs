using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class AppointmentService
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; }

    public int ServiceCatalogItemId { get; set; }
    public ServiceCatalogItem ServiceCatalogItem { get; set; }
}