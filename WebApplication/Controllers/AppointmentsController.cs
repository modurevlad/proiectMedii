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
public class AppointmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AppointmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: api/Appointments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (isAdmin)
        {
            return await _context.Appointments
                .Include(a => a.Car)
                .Include(a => a.Mechanic)
                .ToListAsync();
        }

        if (userId == null)
        {
            return Unauthorized();
        }

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
        if (client == null)
        {
            return new List<Appointment>();
        }

        var userCarIds = await _context.Cars
            .Where(c => c.ClientId == client.Id)
            .Select(c => c.Id)
            .ToListAsync();

        return await _context.Appointments
            .Include(a => a.Car)
            .Include(a => a.Mechanic)
            .Where(a => userCarIds.Contains(a.CarId))
            .ToListAsync();
    }

    // GET: api/Appointments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Car)
            .ThenInclude(c => c.Client)
            .Include(a => a.Mechanic)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || appointment.Car.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        return appointment;
    }

    // PUT: api/Appointments/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
    {
        if (id != appointment.Id)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var originalAppointment = await _context.Appointments
            .Include(a => a.Car)
            .ThenInclude(c => c.Client)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (originalAppointment == null)
        {
            return NotFound();
        }

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || originalAppointment.Car.ClientId != client.Id)
            {
                return Forbid();
            }
            // Preserve status for non-admin users
            appointment.Status = originalAppointment.Status;
        }

        originalAppointment.AppointmentDate = appointment.AppointmentDate;
        originalAppointment.Status = appointment.Status;
        originalAppointment.CarId = appointment.CarId;
        originalAppointment.MechanicId = appointment.MechanicId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AppointmentExists(id))
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

    // POST: api/Appointments
    [HttpPost]
    public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        // If user is not admin, force Status to "Pending"
        if (!isAdmin)
        {
            appointment.Status = "Pending";
        }

        // Verify car ownership for non-admin users
        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null)
            {
                return Forbid();
            }

            var car = await _context.Cars.FindAsync(appointment.CarId);
            if (car == null || car.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAppointment", new { id = appointment.Id }, appointment);
    }

    // DELETE: api/Appointments/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Car)
            .ThenInclude(c => c.Client)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && userId != null)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null || appointment.Car.ClientId != client.Id)
            {
                return Forbid();
            }
        }

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AppointmentExists(int id)
    {
        return _context.Appointments.Any(e => e.Id == id);
    }
}

