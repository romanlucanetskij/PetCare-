using System.ComponentModel.DataAnnotations;

namespace PetCarePlus.DTOs;

public record AppointmentRequest(
    [property: Required] int PetId,
    [property: Required] int VetId,
    [property: Required] DateOnly Date,
    [property: Required] TimeOnly Time);

public record AppointmentUpdateRequest(
    [property: RegularExpression("^(pending|approved|completed|cancelled)$")] string? Status,
    string? Diagnosis,
    string? Notes);

public record AppointmentResponse(
    int Id,
    string PetName,
    string VetName,
    DateOnly Date,
    TimeOnly Time,
    string Status,
    string? Diagnosis,
    string? Notes);
