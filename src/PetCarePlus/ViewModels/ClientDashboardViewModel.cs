using PetCarePlus.Models;

namespace PetCarePlus.ViewModels;

public class ClientDashboardViewModel
{
    public IEnumerable<Pet> Pets { get; set; } = Enumerable.Empty<Pet>();
    public IEnumerable<Appointment> Appointments { get; set; } = Enumerable.Empty<Appointment>();
    public IEnumerable<User> AvailableVets { get; set; } = Enumerable.Empty<User>();
}
