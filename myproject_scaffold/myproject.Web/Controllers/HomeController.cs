using Microsoft.AspNetCore.Mvc;

namespace myproject.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
