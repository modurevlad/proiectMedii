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

namespace WebApplication.Pages.Cars
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

        public IList<Car> Car { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            if (isAdmin)
            {
                // Admin sees all cars
                Car = await _context.Cars
                    .Include(c => c.Client).ToListAsync();
            }
            else
            {
                // User sees only their own cars
                if (user != null)
                {
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
                    if (client != null)
                    {
                        Car = await _context.Cars
                            .Include(c => c.Client)
                            .Where(c => c.ClientId == client.Id)
                            .ToListAsync();
                    }
                    else
                    {
                        Car = new List<Car>();
                    }
                }
                else
                {
                    Car = new List<Car>();
                }
            }
        }
    }
}
