using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AppointmentServicesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppointmentServicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/AppointmentServices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentService>>> GetAppointmentServices([FromQuery] int? appointmentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        IQueryable<AppointmentService> query = _context.AppointmentServices
            .Include(a => a.Appointment)
            .ThenInclude(a => a.Car)
            .ThenInclude(c => c.Client)
            .Include(a => a.ServiceCatalogItem);

        if (appointmentId.HasValue)
        {
            query = query.Where(a => a.AppointmentId == appointmentId.Value);
        }

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client != null)
            {
                var userCarIds = await _context.Cars
                    .Where(c => c.ClientId == client.Id)
                    .Select(c => c.Id)
                    .ToListAsync();

                query = query.Where(a => userCarIds.Contains(a.Appointment.CarId));
            }
            else
            {
                return new List<AppointmentService>();
            }
        }

        return await query.ToListAsync();
    }

    // GET: api/AppointmentServices/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentService>> GetAppointmentService(int id)
    {
        var appointmentService = await _context.AppointmentServices
            .Include(a => a.Appointment)
            .ThenInclude(a => a.Car)
            .ThenInclude(c => c.Client)
            .Include(a => a.ServiceCatalogItem)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointmentService == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || appointmentService.Appointment.Car.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        return appointmentService;
    }

    // PUT: api/AppointmentServices/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAppointmentService(int id, AppointmentService appointmentService)
    {
        if (id != appointmentService.Id)
        {
            return BadRequest();
        }

        var original = await _context.AppointmentServices
            .Include(a => a.Appointment)
            .ThenInclude(a => a.Car)
            .ThenInclude(c => c.Client)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (original == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || original.Appointment.Car.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        original.AppointmentId = appointmentService.AppointmentId;
        original.ServiceCatalogItemId = appointmentService.ServiceCatalogItemId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AppointmentServiceExists(id))
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

    // POST: api/AppointmentServices
    [HttpPost]
    public async Task<ActionResult<AppointmentService>> PostAppointmentService(AppointmentService appointmentService)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        // Verify appointment ownership for non-admin users
        if (!isAdmin && userId != null)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Car)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(a => a.Id == appointmentService.AppointmentId);

            if (appointment == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || appointment.Car.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        _context.AppointmentServices.Add(appointmentService);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAppointmentService", new { id = appointmentService.Id }, appointmentService);
    }

    // DELETE: api/AppointmentServices/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointmentService(int id)
    {
        var appointmentService = await _context.AppointmentServices
            .Include(a => a.Appointment)
            .ThenInclude(a => a.Car)
            .ThenInclude(c => c.Client)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointmentService == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || appointmentService.Appointment.Car.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        _context.AppointmentServices.Remove(appointmentService);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AppointmentServiceExists(int id)
    {
        return _context.AppointmentServices.Any(e => e.Id == id);
    }
}

