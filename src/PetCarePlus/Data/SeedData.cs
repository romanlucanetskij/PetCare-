using PetCarePlus.Models;

namespace PetCarePlus.Data;

public static class SeedData
{
    public static void EnsureSeeded(ApplicationDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var admin = new User
        {
            UserName = "admin",
            Email = "admin@petcareplus.ua",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "admin"
        };

        context.Users.Add(admin);
        context.SaveChanges();
    }
}
