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
public class PetsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PetsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PetResponse>>> GetMyPets()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));
        var query = _context.Pets.AsQueryable();

        if (!User.IsInRole("admin"))
        {
            query = query.Where(p => p.OwnerId == userId);
        }

        var pets = await query
            .Select(p => new PetResponse(p.Id, p.Name, p.Species, p.Breed, p.Age))
            .ToListAsync();
        return pets;
    }

    [HttpPost]
    [Authorize(Policy = "ClientOnly")]
    public async Task<ActionResult<PetResponse>> CreatePet(PetRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));

        var pet = new Pet
        {
            Name = request.Name,
            Species = request.Species,
            Breed = request.Breed,
            Age = request.Age,
            OwnerId = userId
        };

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPet), new { id = pet.Id }, new PetResponse(pet.Id, pet.Name, pet.Species, pet.Breed, pet.Age));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PetResponse>> GetPet(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));
        var petQuery = _context.Pets.Where(p => p.Id == id);
        if (!User.IsInRole("admin"))
        {
            petQuery = petQuery.Where(p => p.OwnerId == userId);
        }

        var pet = await petQuery.FirstOrDefaultAsync();
        if (pet is null)
        {
            return NotFound();
        }

        return new PetResponse(pet.Id, pet.Name, pet.Species, pet.Breed, pet.Age);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "ClientOnly")]
    public async Task<IActionResult> UpdatePet(int id, PetRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));
        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId);
        if (pet is null)
        {
            return NotFound();
        }

        pet.Name = request.Name;
        pet.Species = request.Species;
        pet.Breed = request.Breed;
        pet.Age = request.Age;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "ClientOnly")]
    public async Task<IActionResult> DeletePet(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Відсутній ідентифікатор користувача"));
        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId);
        if (pet is null)
        {
            return NotFound();
        }

        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
