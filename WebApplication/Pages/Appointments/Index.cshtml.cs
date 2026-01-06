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
    public class IndexModel : PageModel
    {
        private readonly WebApplication.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(WebApplication.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Appointment> Appointment { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            if (isAdmin)
            {
                // Admin sees all appointments
                Appointment = await _context.Appointments
                    .Include(a => a.Car)
                    .Include(a => a.Mechanic).ToListAsync();
            }
            else
            {
                // User sees only appointments for their own cars
                if (user != null)
                {
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                    if (client != null)
                    {
                        var userCarIds = await _context.Cars
                            .Where(c => c.ClientId == client.Id)
                            .Select(c => c.Id)
                            .ToListAsync();

                        Appointment = await _context.Appointments
                            .Include(a => a.Car)
                            .Include(a => a.Mechanic)
                            .Where(a => userCarIds.Contains(a.CarId))
                            .ToListAsync();
                    }
                    else
                    {
                        Appointment = new List<Appointment>();
                    }
                }
                else
                {
                    Appointment = new List<Appointment>();
                }
            }
        }
    }
}
