using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCarePlus.Data;
using PetCarePlus.DTOs;
using PetCarePlus.Models;
using System.Security.Claims;

namespace PetCarePlus.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppointmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetAppointments()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));
        var role = User.FindFirstValue(ClaimTypes.Role);

        var query = _context.Appointments
            .Include(a => a.Pet)
            .ThenInclude(p => p.Owner)
            .Include(a => a.Vet)
            .AsQueryable();

        query = role switch
        {
            "client" => query.Where(a => a.ClientId == userId),
            "vet" => query.Where(a => a.VetId == userId),
            "admin" => query,
            _ => query.Where(a => false)
        };

        var appointments = await query
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Time)
            .Select(a => new AppointmentResponse(
                a.Id,
                a.Pet.Name,
                a.Vet.UserName,
                a.Date,
                a.Time,
                a.Status,
                a.Diagnosis,
                a.Notes))
            .ToListAsync();

        return appointments;
    }

    [HttpPost]
    [Authorize(Policy = "ClientOnly")]
    public async Task<ActionResult<AppointmentResponse>> CreateAppointment(AppointmentRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));
        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == request.PetId && p.OwnerId == userId);
        if (pet is null)
        {
            return BadRequest("Неможливо записати чужого улюбленця.");
        }

        var vet = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.VetId && u.Role == "vet");
        if (vet is null)
        {
            return BadRequest("Вибраний лікар недоступний.");
        }

        var appointment = new Appointment
        {
            PetId = pet.Id,
            VetId = vet.Id,
            ClientId = userId,
            Date = request.Date,
            Time = request.Time,
            Status = "pending"
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAppointments), new { id = appointment.Id }, new AppointmentResponse(
            appointment.Id,
            pet.Name,
            vet.UserName,
            appointment.Date,
            appointment.Time,
            appointment.Status,
            appointment.Diagnosis,
            appointment.Notes));
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = "VetOnly")]
    public async Task<IActionResult> UpdateAppointment(int id, AppointmentUpdateRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment is null)
        {
            return NotFound();
        }

        if (User.IsInRole("vet") && appointment.VetId != userId)
        {
            return Forbid();
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            appointment.Status = request.Status;
        }

        if (request.Diagnosis is not null)
        {
            appointment.Diagnosis = request.Diagnosis;
        }

        if (request.Notes is not null)
        {
            appointment.Notes = request.Notes;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment is null)
        {
            return NotFound();
        }

        if (User.IsInRole("client") && appointment.ClientId != userId)
        {
            return Forbid();
        }

        appointment.Status = "cancelled";
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
