using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.AppointmentServices
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly WebApplication.Data.ApplicationDbContext _context;

        public EditModel(WebApplication.Data.ApplicationDbContext context)
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

            var appointmentservice =  await _context.AppointmentServices.FirstOrDefaultAsync(m => m.Id == id);
            if (appointmentservice == null)
            {
                return NotFound();
            }
            AppointmentService = appointmentservice;
           ViewData["AppointmentId"] = new SelectList(_context.Appointments, "Id", "Id");
           ViewData["ServiceCatalogItemId"] = new SelectList(_context.ServiceCatalogItems, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(AppointmentService).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentServiceExists(AppointmentService.Id))
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

        private bool AppointmentServiceExists(int id)
        {
            return _context.AppointmentServices.Any(e => e.Id == id);
        }
    }
}
