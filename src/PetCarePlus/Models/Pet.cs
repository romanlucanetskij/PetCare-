using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCarePlus.Models;

public class Pet
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Species { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Breed { get; set; }

    [Range(0, 100)]
    public int Age { get; set; }

    public int OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; } = null!;

    [InverseProperty(nameof(Appointment.Pet))]
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
