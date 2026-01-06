using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.Mechanics
{
    public class IndexModel : PageModel
    {
        private readonly WebApplication.Data.ApplicationDbContext _context;

        public IndexModel(WebApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Mechanic> Mechanic { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Mechanic = await _context.Mechanics.ToListAsync();
        }
    }
}
