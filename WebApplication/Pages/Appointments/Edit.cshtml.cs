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
    public class EditModel : PageModel
    {
        private readonly WebApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(WebApplication.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public bool IsAdmin { get; set; }
        public string OriginalStatus { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Car)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (appointment == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            IsAdmin = User.IsInRole("Admin");

            // Check if user can edit this appointment
            if (!IsAdmin && user != null)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (client == null || appointment.Car.ClientId != client.Id)
                {
                    return Forbid(); // User cannot edit appointments that don't belong to them
                }
            }

            Appointment = appointment;
            OriginalStatus = appointment.Status;

            // Filter cars based on user role
            IQueryable<Car> carsQuery = _context.Cars;
            if (!IsAdmin && user != null)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (client != null)
                {
                    carsQuery = carsQuery.Where(c => c.ClientId == client.Id);
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            IsAdmin = User.IsInRole("Admin");

            // Get original appointment to check ownership and preserve status for non-admins
            var originalAppointment = await _context.Appointments
                .Include(a => a.Car)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(a => a.Id == Appointment.Id);

            if (originalAppointment == null)
            {
                return NotFound();
            }

            // Check if user can edit this appointment
            if (!IsAdmin && user != null)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (client == null || originalAppointment.Car.ClientId != client.Id)
                {
                    return Forbid();
                }
                // Preserve original status for non-admin users
                Appointment.Status = originalAppointment.Status;
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

            // Update the tracked entity instead of attaching a new one
            originalAppointment.AppointmentDate = Appointment.AppointmentDate;
            originalAppointment.Status = Appointment.Status;
            originalAppointment.CarId = Appointment.CarId;
            originalAppointment.MechanicId = Appointment.MechanicId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(Appointment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
