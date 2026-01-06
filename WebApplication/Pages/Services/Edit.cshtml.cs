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

namespace WebApplication.Pages.Services
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
        public ServiceCatalogItem ServiceCatalogItem { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicecatalogitem =  await _context.ServiceCatalogItems.FirstOrDefaultAsync(m => m.Id == id);
            if (servicecatalogitem == null)
            {
                return NotFound();
            }
            ServiceCatalogItem = servicecatalogitem;
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

            _context.Attach(ServiceCatalogItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceCatalogItemExists(ServiceCatalogItem.Id))
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

        private bool ServiceCatalogItemExists(int id)
        {
            return _context.ServiceCatalogItems.Any(e => e.Id == id);
        }
    }
}
