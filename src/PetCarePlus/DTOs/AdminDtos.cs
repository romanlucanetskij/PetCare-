using System.ComponentModel.DataAnnotations;

namespace PetCarePlus.DTOs;

public record PromoteVetRequest([property: Required][property: EmailAddress] string Email);
