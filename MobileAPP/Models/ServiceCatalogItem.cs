namespace MobileAPP.Models;

public class ServiceCatalogItem
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public decimal Price { get; set; }

    public ICollection<AppointmentService>? AppointmentServices { get; set; }
}