using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Models;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WorkShop.Core.Entities; // เพื่อรู้จัก User entity
using WorkShop.Service.Helpers; // เพื่อรู้จัก PasswordHelper
using Microsoft.AspNetCore.Authorization; //เพื่อรู้จัก [AllowAnonymous]

namespace WorkShop.Web.Controllers
{
    public class UserController(IUserService service) : Controller
    {

        public IActionResult Index()
        {
            // หน้านี้ควรกำหนดให้ [Authorize] เพื่อบังคับล็อกอิน
            return View();
        }

        public IActionResult Login()
        {
            //ตรวจสอบก่อนว่าผู้ใช้ "ล็อกอินอยู่แล้ว" หรือไม่
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); //ถ้าล็อกอินอยู่แล้ว: มันจะส่งผู้ใช้กลับไปที่หน้าหลัก(Home / Index) ทันที(เพราะผู้ใช้ไม่จำเป็นต้องเห็นหน้าล็อกอินอีก)
            }
            return View(); //ถ้ายังไม่ล็อกอิน: มันก็จะแสดงหน้า Login.cshtml ตามปกติ
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(string Email, string Password, CancellationToken ct) //"ประมวลผล" การล็อกอินหลังจากที่ผู้ใช้กดปุ่ม "Login" ในฟอร์ม
        {
            var res = await service.LoginhAsync(Email, Password, ct); //รับ Email และ Password ที่ผู้ใช้กรอกมา ส่งไปให้ IUserService (ที่เราทำไว้ก่อนหน้า) ตรวจสอบว่า "อีเมลนี้ และ รหัสผ่านนี้" ถูกต้องและตรงกันในฐานข้อมูลหรือไม่

            if (res == null) 
            {
                // ถ้า res เป็น null = ล็อกอินไม่สำเร็จ (ไม่พบผู้ใช้ หรือ รหัสผ่านผิด)
                ViewData["ErrorMessage"] = "Invalid email or password.";
                return View("Login"); // กลับไปหน้า Login พร้อมแสดงข้อความเตือน
            }

            // ถ้า res ไม่ใช่ null = ล็อกอินสำเร็จ
            // สร้าง Claims (ข้อมูลที่จะเก็บไว้ใน Cookie ว่าผู้ใช้คือใคร)
            var claims = new List<Claim>
            {
                //เก็บ displayname ไว้ เป็นชื่อที่แสดงผล (User.Identity.Name) ในหน้าเว็บ
                new Claim(ClaimTypes.Name, res.DisplayName),

                // เก็บ ID ผู้ใช้ (จากคอลัมน์ UserId ที่เราดูจากรูป)
                new Claim(ClaimTypes.NameIdentifier, res.UserId.ToString()),
                
                // (แนะนำ) เก็บ Email แยกไว้ใน Claim ประเภท Email ด้วย
                new Claim(ClaimTypes.Email, res.Email),

                // (Optional) ถ้าคุณมี Role ก็เพิ่มตรงนี้
                // new Claim(ClaimTypes.Role, res.RoleName), 
            };

            //"โอเค ฉันตรวจสอบคนนี้แล้ว เขาล็อกอินผ่าน" และสั่งให้ระบบ สร้าง Cookie ที่เข้ารหัสไว้ในเครื่องของผู้ใช้
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); 

            // สั่งให้ระบบ Sign In (สร้าง Cookie)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            //ส่งผู้ใช้ไปยังหน้าหลักหลังจากล็อกอินสำเร็จ
            return RedirectToAction("Index", "Home");
        }

        // Action สำหรับสร้าง Hash (สำหรับทดสอบ)
        //[AllowAnonymous] // อนุญาตให้เข้าถึงได้แม้ยังไม่ล็อกอิน
        //public IActionResult GetHash(string id)
        //{
            // "id" คือรหัสผ่านที่เราอยาก Hash
            //if (string.IsNullOrEmpty(id))
           // {
           //     return Content("Please provide a password in the URL. Example: /User/GetHash?id=123456"); //รหัสผ่านเดิม = J8c3o7qjbl5YYS0vGwVQ0g==
           // }

           // var passwordHelper = new PasswordHelper();
            //string hashedPassword = passwordHelper.HashPassword(id);

            // คืนค่า Hash ที่ได้ออกมาเป็น Text
           // return Content(hashedPassword);
       // }

        public async Task<IActionResult> Logout()
        {
            // สั่งให้ระบบ Sign Out (ลบ Cookie ยืนยันตัวตน)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // ส่งผู้ใช้กลับไปหน้า Login
            return RedirectToAction("Login", "User");
        }
    }
}