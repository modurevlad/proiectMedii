using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CarsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CarsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: api/Cars
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetCars()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (isAdmin)
        {
            return await _context.Cars.Include(c => c.Client).ToListAsync();
        }

        if (userId == null)
        {
            return Unauthorized();
        }

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
        if (client == null)
        {
            return new List<Car>();
        }

        return await _context.Cars
            .Include(c => c.Client)
            .Where(c => c.ClientId == client.Id)
            .ToListAsync();
    }

    // GET: api/Cars/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Car>> GetCar(int id)
    {
        var car = await _context.Cars
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || car.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        return car;
    }

    // PUT: api/Cars/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCar(int id, Car car)
    {
        if (id != car.Id)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var originalCar = await _context.Cars.FindAsync(id);
        if (originalCar == null)
        {
            return NotFound();
        }

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || originalCar.ClientId != client.Id)
            {
                return Forbid();
            }
            // Preserve ClientId for non-admin users
            car.ClientId = originalCar.ClientId;
        }

        originalCar.Brand = car.Brand;
        originalCar.Model = car.Model;
        originalCar.Year = car.Year;
        originalCar.PlateNumber = car.PlateNumber;
        if (isAdmin)
        {
            originalCar.ClientId = car.ClientId;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CarExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Cars
    [HttpPost]
    public async Task<ActionResult<Car>> PostCar(Car car)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null)
            {
                return Forbid();
            }
            // Auto-assign ClientId for non-admin users
            car.ClientId = client.Id;
        }

        _context.Cars.Add(car);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCar", new { id = car.Id }, car);
    }

    // DELETE: api/Cars/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || car.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CarExists(int id)
    {
        return _context.Cars.Any(e => e.Id == id);
    }
}

