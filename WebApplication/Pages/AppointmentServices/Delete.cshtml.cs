using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.AppointmentServices
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        
        private readonly WebApplication.Data.ApplicationDbContext _context;

        public DeleteModel(WebApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AppointmentService AppointmentService { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentservice = await _context.AppointmentServices.FirstOrDefaultAsync(m => m.Id == id);

            if (appointmentservice == null)
            {
                return NotFound();
            }
            else
            {
                AppointmentService = appointmentservice;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentservice = await _context.AppointmentServices.FindAsync(id);
            if (appointmentservice != null)
            {
                AppointmentService = appointmentservice;
                _context.AppointmentServices.Remove(AppointmentService);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
