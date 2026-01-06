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

            if (IsAdmin)
            {
                // Admin can choose any client
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            }
            else
            {
                // User: automatically set to their own client
                if (user != null)
                {
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                    if (client != null)
                    {
                        Car = new Car { ClientId = client.Id };
                    }
                    else
                    {
                        // User doesn't have a client record - shouldn't happen but handle it
                        return Forbid();
                    }
                }
                else
                {
                    return Forbid();
                }
            }

            return Page();
        }

        [BindProperty]
        public Car Car { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            IsAdmin = User.IsInRole("Admin");

            // If user is not admin, force ClientId to their own client
            if (!IsAdmin && user != null)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (client != null)
                {
                    Car.ClientId = client.Id; // Override any value sent from client
                }
                else
                {
                    return Forbid();
                }
            }

            if (!ModelState.IsValid)
            {
                if (IsAdmin)
                {
                    ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", Car.ClientId);
                }
                return Page();
            }

            _context.Cars.Add(Car);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
