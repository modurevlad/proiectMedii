using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ServiceCatalogItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ServiceCatalogItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/ServiceCatalogItems
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceCatalogItem>>> GetServiceCatalogItems()
    {
        // Read-only endpoint - all authenticated users can access
        return await _context.ServiceCatalogItems.ToListAsync();
    }

    // GET: api/ServiceCatalogItems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceCatalogItem>> GetServiceCatalogItem(int id)
    {
        var serviceCatalogItem = await _context.ServiceCatalogItems.FindAsync(id);

        if (serviceCatalogItem == null)
        {
            return NotFound();
        }

        return serviceCatalogItem;
    }
}

