using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.Services
{
    public class DetailsModel : PageModel
    {
        private readonly WebApplication.Data.ApplicationDbContext _context;

        public DetailsModel(WebApplication.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
