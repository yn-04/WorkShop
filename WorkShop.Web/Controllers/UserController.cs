using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Models;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WorkShop.Core.Entities; //User entity
using WorkShop.Service.Helpers; //PasswordHelper
using Microsoft.AspNetCore.Authorization; //[AllowAnonymous]
using WorkShop.Infrastructure.Data; //  DbContext
using Microsoft.EntityFrameworkCore; // EF Core

namespace WorkShop.Web.Controllers
{
    public class UserController(IUserService service, WorkShopDbContext db) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        //ทำงานหลังจากที่ผู้ใช้กรอกอีเมล/รหัสผ่าน แล้วกดปุ่มLogin
        public async Task<IActionResult> UserLogin(string Email, string Password, CancellationToken ct)
        {
            var res = await service.LoginhAsync(Email, Password, ct); //เช็กว่าอีเมลและรหัสผ่านนี้ถูกต้องหรือไม่
                                                                      //res ที่ได้กลับมา คือ Userพ่วงUserRoles มาด้วย

            if (res == null) //รหัสผิด หรือไม่มีผู้ใช้
            {
                ViewData["ErrorMessage"] = "Invalid email or password.";
                return View("Login");
            }

            var claims = new List<Claim> //ล้อกอินผ่านสร้าง claims
            {
                new Claim(ClaimTypes.Name, res.DisplayName), // (เก็บ DisplayName สำหรับ Navbar)
                new Claim(ClaimTypes.NameIdentifier, res.UserId.ToString()), // (เก็บ UserId)
                new Claim(ClaimTypes.Email, res.Email), // (เก็บ Email)
            };

            // ดึง RoleId ทั้งหมดที่ผู้ใช้มี (จาก User Entity ที่ Include มา)
            var userRoleIds = res.UserRoles.Select(ur => ur.RoleId).ToList();
            //คำนวณสิทธิ์ล่วงหน้า
            var permissions = await db.RoleMenuPermissions//ดึงข้อมูลจากตาราง RoleMenuPermission
                .Include(p => p.Menu) // Join ตาราง Menu
                .Where(p => userRoleIds.Contains(p.RoleId) && p.Menu.IsActive == 1)  //กรองเฉพาะ RoleId ที่ผู้ใช้มี และเมนูที่ Active
                .Select(p => p.Menu.MenuCode) // สิทธิ์ คือ MenuCode

                .Distinct() //เอาเฉพาะmenucodeที่ไม่ซ้ำกัน
                .ToListAsync(ct); //เอาผลลัพธ์ทั้งหมดที่ได้มาใส่ไว้ในlist async ไม่หยุดรอเฉยๆ ไปทำงานอย่างอื่นรอก่อน
                                  //ct คือ ปุ่มยกเลิก(CancellationToken) ถ้าผู้ใช้ยกเลิกหรือกดปิดเบาเซอร์จะหยุดการทำงาน

            //มันจะวนลูปสิทธิ์(MenuCode) ที่หาได้ทั้งหมด แล้วเก็บลง claims
            foreach (var permission in permissions)
            {
                if (!string.IsNullOrEmpty(permission)) //ป้องกัน MenuCode ที่เป็น NULL
                {
                    //ตั้งชื่อ Claim ชนิดใหม่ว่า Permission
                    claims.Add(new Claim("Permission", permission));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // --- เครื่องมือสร้าง Hash (แบบปลอดภัย ใช้ Form POST) ---
        [AllowAnonymous]
        public IActionResult GetHash()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult GetHash(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                ViewData["ErrorMessage"] = "Please enter a password to hash.";
                return View();
            }

            var passwordHelper = new PasswordHelper();
            string hashedPassword = passwordHelper.HashPassword(password);

            ViewData["HashedPassword"] = hashedPassword;
            return View();
        }
    }
}