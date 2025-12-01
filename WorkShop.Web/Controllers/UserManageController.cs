using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Models;

namespace WorkShop.Web.Controllers
{
    // กำหนดสิทธิ์การเข้าถึงเฉพาะผู้ที่มี Policy "CanManageUsers"
    [Authorize(Policy = "CanManageUsers")]
    public class UserManageController(IUserManageService service) : Controller
    {
        // Helper Method: ดึง User ID ของคนที่ล็อกอินอยู่ (คืนค่าเป็น long)
        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
            {
                return userId;
            }
            return 0;
        }

        // ==========================================
        // ✅ 1. แก้ไข Index (เตรียมหน้าจอหลักรอ AJAX)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            // เตรียม Dropdown Role สำหรับหน้า Search
            var roles = await service.GetAllRolesAsync(ct);
            ViewBag.RoleList = new SelectList(roles, "RoleId", "RoleName");

            // ส่ง Model เปล่าไปให้ View เพื่อป้องกัน Null Reference ตอนสร้าง Form
            return View(new UserSearchModel());
        }

        //Ajax
        [HttpGet]
        public async Task<IActionResult> GetUserList(UserSearchModel searchModel, CancellationToken ct)
        {
            // Map ข้อมูลลงตัวแปร searchModel
            searchModel ??= new UserSearchModel();

            // เรียก Service ค้นหาข้อมูลตามเงื่อนไขที่ส่งมา
            var result = await service.SearchUsersAsync(searchModel, ct);

            // ส่งคืน Partial View ส่งไปแค่HTML ของตาราง
            return PartialView("_UserListPartial", result);
        }

        // หน้าสร้างผู้ใช้ใหม่ (Create - GET)
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var roles = await service.GetAllRolesAsync(ct);
            ViewBag.RoleList = new MultiSelectList(roles, "RoleId", "RoleName");
            return View(new UserFormModel { IsActive = 1 });
        }

        // บันทึกการสร้างผู้ใช้ (Create - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormModel model, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                if (await service.CheckEmailExistsAsync(model.Email, null, ct))
                {
                    ModelState.AddModelError("Email", "This email is already in use.");
                    var roles = await service.GetAllRolesAsync(ct);
                    ViewBag.RoleList = new MultiSelectList(roles, "RoleId", "RoleName", model.RoleIds);
                    return View(model);
                }

                await service.CreateUserAsync(model, GetCurrentUserId(), ct);
                return RedirectToAction(nameof(Index));
            }

            var allRoles = await service.GetAllRolesAsync(ct);
            ViewBag.RoleList = new MultiSelectList(allRoles, "RoleId", "RoleName", model.RoleIds);
            return View(model);
        }

        // หน้าแก้ไขผู้ใช้ (Edit - GET)
        [HttpGet]
        public async Task<IActionResult> Edit(long id, CancellationToken ct)
        {
            var model = await service.GetUserForEditAsync(id, ct);
            if (model == null) return NotFound();

            var roles = await service.GetAllRolesAsync(ct);
            ViewBag.RoleList = new MultiSelectList(roles, "RoleId", "RoleName", model.RoleIds);

            return View(model);
        }

        // บันทึกการแก้ไขผู้ใช้ (Edit - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormModel model, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.Remove("Password");
            }

            if (ModelState.IsValid)
            {
                if (await service.CheckEmailExistsAsync(model.Email, model.UserId, ct))
                {
                    ModelState.AddModelError("Email", "This email is already in use by another user.");
                    var roles = await service.GetAllRolesAsync(ct);
                    ViewBag.RoleList = new MultiSelectList(roles, "RoleId", "RoleName", model.RoleIds);
                    return View(model);
                }

                await service.UpdateUserAsync(model, GetCurrentUserId(), ct);
                return RedirectToAction(nameof(Index));
            }

            var allRoles = await service.GetAllRolesAsync(ct);
            ViewBag.RoleList = new MultiSelectList(allRoles, "RoleId", "RoleName", model.RoleIds);
            return View(model);
        }

        // ลบผู้ใช้ (Delete - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id, CancellationToken ct)
        {
            await service.DeleteUserAsync(id, ct);
            return RedirectToAction(nameof(Index));
        }
    }
}