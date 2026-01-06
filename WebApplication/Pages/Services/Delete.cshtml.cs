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

namespace WebApplication.Pages.Services
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
        public ServiceCatalogItem ServiceCatalogItem { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicecatalogitem = await _context.ServiceCatalogItems.FirstOrDefaultAsync(m => m.Id == id);

            if (servicecatalogitem == null)
            {
                return NotFound();
            }
            else
            {
                ServiceCatalogItem = servicecatalogitem;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicecatalogitem = await _context.ServiceCatalogItems.FindAsync(id);
            if (servicecatalogitem != null)
            {
                ServiceCatalogItem = servicecatalogitem;
                _context.ServiceCatalogItems.Remove(ServiceCatalogItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
