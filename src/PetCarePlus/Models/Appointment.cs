using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCarePlus.Models;

public class Appointment
{
    public int Id { get; set; }

    public int PetId { get; set; }

    [ForeignKey(nameof(PetId))]
    public Pet Pet { get; set; } = null!;

    public int VetId { get; set; }

    [ForeignKey(nameof(VetId))]
    public User Vet { get; set; } = null!;

    public int ClientId { get; set; }

    [ForeignKey(nameof(ClientId))]
    public User Client { get; set; } = null!;

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public TimeOnly Time { get; set; }

    [Required]
    [RegularExpression("^(pending|approved|completed|cancelled)$")]
    public string Status { get; set; } = "pending";

    public string? Diagnosis { get; set; }

    public string? Notes { get; set; }
}
