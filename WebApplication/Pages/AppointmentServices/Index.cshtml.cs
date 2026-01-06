using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.AppointmentServices
{
    public class IndexModel : PageModel
    {
        private readonly WebApplication.Data.ApplicationDbContext _context;

        public IndexModel(WebApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<AppointmentService> AppointmentService { get;set; } = default!;

        public async Task OnGetAsync()
        {
            AppointmentService = await _context.AppointmentServices
                .Include(a => a.Appointment)
                .Include(a => a.ServiceCatalogItem)
                .ToListAsync();

        }
    }
}
