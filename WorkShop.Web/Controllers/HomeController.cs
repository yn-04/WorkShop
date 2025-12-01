using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WorkShop.Web.Models;

using Microsoft.AspNetCore.Authorization; 

namespace WorkShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //Authorize ทั้ง Controller นี้ โดยใช้ Policy CanViewHome
        [Authorize(Policy = "CanViewHome")] //ต้องมีสิทธิ์ในหน้า Home
        public IActionResult Index()
        {
            return View();
        }

        [Authorize] //แค่ล็อกอินก็พอ ไม่ต้องมี Policy
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}