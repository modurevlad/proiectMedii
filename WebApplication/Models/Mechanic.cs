using System.ComponentModel.DataAnnotations;
using EnvironmentName = Microsoft.AspNetCore.Hosting.EnvironmentName;

namespace WebApplication.Models;

public class Mechanic
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Specialty { get; set; }

    // legătură cu Identity user
    [Display(Name= "User ID")]
    public string? UserId { get; set; }
}