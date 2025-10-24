using Microsoft.AspNetCore.Mvc;

namespace PetCarePlus.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    public IActionResult Error() => View("~/Views/Shared/Error.cshtml");
}
