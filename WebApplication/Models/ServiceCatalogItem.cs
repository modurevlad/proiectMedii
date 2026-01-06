using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class ServiceCatalogItem
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Range(1, 10000)]
    public decimal Price { get; set; }

    public ICollection<AppointmentService>? AppointmentServices { get; set; }
}