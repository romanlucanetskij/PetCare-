using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCarePlus.Data;
using PetCarePlus.DTOs;
using PetCarePlus.Models;

namespace PetCarePlus.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("vets")]
    public async Task<ActionResult<IEnumerable<object>>> GetVets()
    {
        var vets = await _context.Users
            .Where(u => u.Role == "vet")
            .Select(u => new { u.Id, u.UserName, u.Email })
            .ToListAsync();
        return Ok(vets);
    }

    [HttpPost("vets")]
    public async Task<ActionResult> AddVet(PromoteVetRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null)
        {
            return NotFound("Користувача не знайдено.");
        }

        user.Role = "vet";
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetStats()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalPets = await _context.Pets.CountAsync();
        var upcomingAppointments = await _context.Appointments.CountAsync(a => a.Date >= DateOnly.FromDateTime(DateTime.Today));

        return Ok(new
        {
            totalUsers,
            totalPets,
            upcomingAppointments
        });
    }
}
