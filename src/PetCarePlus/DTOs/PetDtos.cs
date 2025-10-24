using System.ComponentModel.DataAnnotations;

namespace PetCarePlus.DTOs;

public record PetRequest(
    [property: Required] string Name,
    [property: Required] string Species,
    string? Breed,
    [property: Range(0, 100)] int Age);

public record PetResponse(int Id, string Name, string Species, string? Breed, int Age);
