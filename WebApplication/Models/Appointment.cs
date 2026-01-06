using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class Appointment
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Appointment Date")]
    public DateTime AppointmentDate { get; set; }

    [Required]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Pending";

    // FK către Car
    [Display(Name = "Car")]
    public int CarId { get; set; }
    public Car? Car { get; set; }

    // FK către Mechanic
    [Display(Name = "Mechanic")]
    public int MechanicId { get; set; }
    public Mechanic? Mechanic { get; set; }

    public ICollection<AppointmentService>? AppointmentServices { get; set; }
}