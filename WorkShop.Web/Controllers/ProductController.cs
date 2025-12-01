using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkShop.Web.Controllers
{
    //Authorize ทั้ง Controller นี้ โดยใช้ Policy CanManageOrders
    [Authorize(Policy = "CanManageProducts")] //ต้องมีสิทธิ์ในหน้า Product
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}