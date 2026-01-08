namespace MobileAPP.Models;

public class Appointment
{
    public int Id { get; set; }
    
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "Pending";
    public int CarId { get; set; }
    public Car? Car { get; set; }
    public int MechanicId { get; set; }
    public Mechanic? Mechanic { get; set; }
    public ICollection<AppointmentService>? AppointmentServices { get; set; }
}