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
public class ClientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ClientsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: api/Clients
    [HttpGet]
    public async Task<ActionResult<Client>> GetCurrentClient()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
        
        if (client == null)
        {
            return NotFound();
        }

        return client;
    }

    // GET: api/Clients/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetClient(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        
        if (client == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && userId != null)
        {
            if (client.UserId != userId)
            {
                return Forbid();
            }
        }

        return client;
    }

    // POST: api/Clients
    [HttpPost]
    public async Task<ActionResult<Client>> PostClient(Client client)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        // Check if client already exists for this user
        var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
        if (existingClient != null)
        {
            return BadRequest(new { error = "Client profile already exists for this user" });
        }

        // Auto-assign UserId for non-admin users
        client.UserId = userId;

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetClient", new { id = client.Id }, client);
    }

    // PUT: api/Clients/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutClient(int id, Client client)
    {
        if (id != client.Id)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var originalClient = await _context.Clients.FindAsync(id);
        if (originalClient == null)
        {
            return NotFound();
        }

        if (!isAdmin && userId != null)
        {
            if (originalClient.UserId != userId)
            {
                return Forbid();
            }
            // Preserve UserId for non-admin users
            client.UserId = originalClient.UserId;
        }

        originalClient.Name = client.Name;
        originalClient.Phone = client.Phone;
        originalClient.Email = client.Email;
        if (isAdmin)
        {
            originalClient.UserId = client.UserId;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClientExists(id))
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

    private bool ClientExists(int id)
    {
        return _context.Clients.Any(e => e.Id == id);
    }
}

