using System.ComponentModel.DataAnnotations;

namespace PetCarePlus.DTOs;

public record RegisterRequest(
    [property: Required][property: EmailAddress] string Email,
    [property: Required][property: MinLength(3)] string UserName,
    [property: Required][property: MinLength(6)] string Password,
    [property: Required][property: RegularExpression("^(client|vet)$")] string Role);

public record LoginRequest(
    [property: Required][property: EmailAddress] string Email,
    [property: Required] string Password);

public record AuthResponse(string Token, string Role, string UserName);
