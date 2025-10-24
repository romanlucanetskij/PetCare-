using PetCarePlus.Models;

namespace PetCarePlus.ViewModels;

public class VetDashboardViewModel
{
    public IEnumerable<Appointment> Appointments { get; set; } = Enumerable.Empty<Appointment>();
}
