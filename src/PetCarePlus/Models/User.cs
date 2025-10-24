using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCarePlus.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(client|vet|admin)$")]
    public string Role { get; set; } = "client";

    [InverseProperty(nameof(Pet.Owner))]
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();

    [InverseProperty(nameof(Appointment.Vet))]
    public ICollection<Appointment> AppointmentsAsVet { get; set; } = new List<Appointment>();

    [InverseProperty(nameof(Appointment.Client))]
    public ICollection<Appointment> AppointmentsAsClient { get; set; } = new List<Appointment>();
}
