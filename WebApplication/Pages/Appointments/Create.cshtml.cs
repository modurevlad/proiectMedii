using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.Appointments
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly WebApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(WebApplication.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public bool IsAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            IsAdmin = User.IsInRole("Admin");

            // Filter cars based on user role
            IQueryable<Car> carsQuery = _context.Cars;
            if (!IsAdmin && user != null)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (client != null)
                {
                    carsQuery = carsQuery.Where(c => c.ClientId == client.Id);
                }
                else
                {
                    carsQuery = carsQuery.Where(c => false); // No cars if no client
                }
            }

            var cars = await carsQuery.Select(c => new { 
                c.Id, 
                DisplayText = $"{c.Brand} {c.Model} ({c.Year}) - {c.PlateNumber}" 
            }).ToListAsync();

            ViewData["CarId"] = new SelectList(cars, "Id", "DisplayText");
            ViewData["MechanicId"] = new SelectList(_context.Mechanics, "Id", "Name");
            
            if (IsAdmin)
            {
                ViewData["Status"] = new SelectList(new[] { "Pending", "Confirmed", "InProgress", "Completed", "Cancelled" });
            }

            return Page();
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            IsAdmin = User.IsInRole("Admin");

            // If user is not admin, force Status to "Pending"
            if (!IsAdmin)
            {
                Appointment.Status = "Pending";
            }

            if (!ModelState.IsValid)
            {
                // Filter cars based on user role
                IQueryable<Car> carsQuery = _context.Cars;
                if (!IsAdmin && user != null)
                {
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                    if (client != null)
                    {
                        carsQuery = carsQuery.Where(c => c.ClientId == client.Id);
                    }
                    else
                    {
                        carsQuery = carsQuery.Where(c => false);
                    }
                }

                var cars = await carsQuery.Select(c => new { 
                    c.Id, 
                    DisplayText = $"{c.Brand} {c.Model} ({c.Year}) - {c.PlateNumber}" 
                }).ToListAsync();

                ViewData["CarId"] = new SelectList(cars, "Id", "DisplayText", Appointment.CarId);
                ViewData["MechanicId"] = new SelectList(_context.Mechanics, "Id", "Name", Appointment.MechanicId);
                
                if (IsAdmin)
                {
                    ViewData["Status"] = new SelectList(new[] { "Pending", "Confirmed", "InProgress", "Completed", "Cancelled" }, Appointment.Status);
                }
                
                return Page();
            }

            _context.Appointments.Add(Appointment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
