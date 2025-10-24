using PetCarePlus.Models;

namespace PetCarePlus.ViewModels;

public class AdminStatsViewModel
{
    public int TotalUsers { get; set; }
    public int TotalPets { get; set; }
    public int UpcomingAppointments { get; set; }
    public IEnumerable<User> Vets { get; set; } = Enumerable.Empty<User>();
}
