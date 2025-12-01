using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorkShop.Infrastructure.Data;
using WorkShop.Service.Interfaces;
using WorkShop.Web.Models; // ตรวจสอบว่า MenuViewModel อยู่ namespace นี้จริงไหม ถ้าไม่อยู่ให้แก้เป็น WorkShop.Service.Models

namespace WorkShop.Service.Services
{
    public class MenuService(WorkShopDbContext db) : IMenuService
    {
        public async Task<List<MenuViewModel>> GetAllowedMenusAsync(ClaimsPrincipal user)
        {
            // 1. ตรวจสอบว่า Login หรือยัง
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return new List<MenuViewModel>();
            }

            // 2. ดึง UserId จาก Claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return new List<MenuViewModel>();
            }

            // 3. Query Database: Join 4 ตาราง (UserRole -> Role -> RoleMenuPermission -> Menu)
            var menus = await (from ur in db.UserRoles
                               join r in db.Roles on ur.RoleId equals r.RoleId // ✅ 1. เพิ่มการ Join ตาราง Role
                               join rmp in db.RoleMenuPermissions on ur.RoleId equals rmp.RoleId
                               join m in db.Menus on rmp.MenuId equals m.MenuId
                               where ur.UserId == userId
                                     && m.IsActive == 1 // เมนูต้องเปิดใช้งาน
                                     && r.IsActive == 1 // ✅ 2. เพิ่มเงื่อนไข Role ต้องเปิดใช้งาน (Active) อยู่เท่านั้น
                               orderby m.SortOrder
                               select new MenuViewModel
                               {
                                   MenuName = m.MenuName,
                                   ControllerName = m.ControllerName,
                                   ActionName = m.ActionName,
                                   // เช็คค่า Null ของ IconClass
                                   IconClass = string.IsNullOrEmpty(m.IconClass) ? "ri-checkbox-blank-circle-line" : m.IconClass,
                                   SortOrder = m.SortOrder
                               })
                               .Distinct() // ป้องกันเมนูซ้ำ
                               .ToListAsync();

            return menus;
        }
    }
}