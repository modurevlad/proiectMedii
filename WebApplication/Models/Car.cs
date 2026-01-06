using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class Car
{
    public int Id { get; set; }

    [Required]
    public string Brand { get; set; }

    [Required]
    public string Model { get; set; }

    [Required]
    [Display(Name = "Year")]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int Year { get; set; }

    [Required]
    [Display(Name="Plate Number")]
    public string PlateNumber { get; set; }

    // FK
    [Display(Name = "Client ID")]
    public int ClientId { get; set; }

    // Navigation property
    public Client? Client { get; set; }
}