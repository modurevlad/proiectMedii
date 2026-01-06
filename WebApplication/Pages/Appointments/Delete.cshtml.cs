using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.Appointments
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly WebApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(WebApplication.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Car)
                .ThenInclude(c => c.Client)
                .Include(a =>a.Mechanic)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            // Check if user can delete this appointment
            if (!isAdmin && user != null)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (client == null || appointment.Car.ClientId != client.Id)
                {
                    return Forbid(); // User cannot delete appointments that don't belong to them
                }
            }

            Appointment = appointment;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Car)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            // Check if user can delete this appointment
            if (!isAdmin && user != null)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (client == null || appointment.Car.ClientId != client.Id)
                {
                    return Forbid();
                }
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
