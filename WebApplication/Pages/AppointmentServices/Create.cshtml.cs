using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.AppointmentServices
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly WebApplication.Data.ApplicationDbContext _context;

        public CreateModel(WebApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["AppointmentId"] = new SelectList(_context.Appointments, "Id", "Id");
        ViewData["ServiceCatalogItemId"] = new SelectList(_context.ServiceCatalogItems, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public AppointmentService AppointmentService { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.AppointmentServices.Add(AppointmentService);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
