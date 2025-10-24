using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCarePlus.Data;
using PetCarePlus.ViewModels;
using System.Security.Claims;

namespace PetCarePlus.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Policy = "ClientOnly")]
    public async Task<IActionResult> Client()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));
        var pets = await _context.Pets.Where(p => p.OwnerId == userId).ToListAsync();
        var appointments = await _context.Appointments
            .Include(a => a.Pet)
            .Include(a => a.Vet)
            .Where(a => a.ClientId == userId)
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Time)
            .ToListAsync();

        var vets = await _context.Users.Where(u => u.Role == "vet").ToListAsync();
        var model = new ClientDashboardViewModel
        {
            Pets = pets,
            Appointments = appointments,
            AvailableVets = vets
        };

        return View("Client", model);
    }

    [Authorize(Policy = "VetOnly")]
    public async Task<IActionResult> Vet()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));
        var appointments = await _context.Appointments
            .Include(a => a.Pet)
            .ThenInclude(p => p.Owner)
            .Where(a => a.VetId == userId)
            .OrderBy(a => a.Date)
            .ThenBy(a => a.Time)
            .ToListAsync();

        var model = new VetDashboardViewModel
        {
            Appointments = appointments
        };

        return View("Vet", model);
    }

    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Admin()
    {
        var vets = await _context.Users.Where(u => u.Role == "vet").ToListAsync();
        var stats = new AdminStatsViewModel
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalPets = await _context.Pets.CountAsync(),
            UpcomingAppointments = await _context.Appointments.CountAsync(a => a.Date >= DateOnly.FromDateTime(DateTime.Today)),
            Vets = vets
        };

        return View("Admin", stats);
    }
}
