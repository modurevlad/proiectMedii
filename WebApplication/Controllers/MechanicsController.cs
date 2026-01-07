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
public class MechanicsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MechanicsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Mechanics
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mechanic>>> GetMechanics()
    {
        // Read-only endpoint - all authenticated users can access
        return await _context.Mechanics.ToListAsync();
    }

    // GET: api/Mechanics/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Mechanic>> GetMechanic(int id)
    {
        var mechanic = await _context.Mechanics.FindAsync(id);

        if (mechanic == null)
        {
            return NotFound();
        }

        return mechanic;
    }
}

