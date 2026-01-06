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

namespace WebApplication.Pages.Cars
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
        public Car Car { get; set; } = default!;

        public bool IsAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (car == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            IsAdmin = User.IsInRole("Admin");

            // Check if user can edit this car
            if (!IsAdmin && user != null)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (client == null || car.ClientId != client.Id)
                {
                    return Forbid(); // User cannot edit cars that don't belong to them
                }
            }

            Car = car;

            if (IsAdmin)
            {
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", Car.ClientId);
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            IsAdmin = User.IsInRole("Admin");

            // Get original car to check ownership and preserve ClientId for non-admins
            var originalCar = await _context.Cars
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == Car.Id);

            if (originalCar == null)
            {
                return NotFound();
            }

            // Check if user can edit this car
            if (!IsAdmin && user != null)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (client == null || originalCar.ClientId != client.Id)
                {
                    return Forbid();
                }
                // Preserve original ClientId for non-admin users
                Car.ClientId = originalCar.ClientId;
            }

            if (!ModelState.IsValid)
            {
                if (IsAdmin)
                {
                    ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", Car.ClientId);
                }
                return Page();
            }

            // Update the tracked entity instead of attaching a new one
            originalCar.Brand = Car.Brand;
            originalCar.Model = Car.Model;
            originalCar.Year = Car.Year;
            originalCar.PlateNumber = Car.PlateNumber;
            if (IsAdmin)
            {
                originalCar.ClientId = Car.ClientId;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(Car.Id))
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

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
