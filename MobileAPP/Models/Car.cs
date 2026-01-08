namespace MobileAPP.Models;

public class Car
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string PlateNumber { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }
}