using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkShop.Web.Controllers
{
    //Authorize ทั้ง Controller นี้ โดยใช้ Policy CanManageOrders
    [Authorize(Policy = "CanManageOrders")] //ต้องมีสิทธิ์ในหน้า Order
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}