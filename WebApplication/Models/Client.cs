using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models;

public class Client
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Phone { get; set; }

    [Required]
    public string Email { get; set; }

    // legătură cu userul din Identity (pentru mobile)
    [Display(Name = "User ID")]
    public string? UserId { get; set; }
}