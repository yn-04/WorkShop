/*
using Microsoft.AspNetCore.Authorization; // ✅ เพิ่ม namespace นี้
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Models;

namespace WorkShop.Web.Controllers
{
    //Authorize Policy "CanManageRoles" (AD002)
    [Authorize(Policy = "CanManageRoles")]
    public class RoleManageController(IRoleManageService service) : Controller
    {
        private int GetCurrentUserId() //ดึง UserId ของคน Login ใช้เพื่อส่งเข้าไปใน Service เวลาที่มีการ สร้าง/แก้ไข/ลบ ข้อมูล เพื่อบันทึกว่า ใครเป็นคนทำรายการนี้
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)) 
            {
                return userId;
            }
            return 0;
        }

        //แสดงรายการ Role ทั้งหมด หรือตามคำค้นหา (Keyword) แสดงรายการ Role ทั้งหมด หรือตามคำค้นหา (Keyword)
        public async Task<IActionResult> Index(string? keyword, int pageNo = 1, CancellationToken ct = default)
        {
            // สร้าง Model สำหรับส่งไป Service
            var searchModel = new RoleSearchModel
            {
                Keyword = keyword,
                PageNo = pageNo,
                PageSize = 5 
            };

            // เรียก Service เพื่อค้นหาและแบ่งหน้า(Pagination)
            // (Service จะคืนค่า RoleSearchModel ที่มี Results และ TotalRecords กลับมา)
            var model = await service.SearchRolesAsync(searchModel, ct);

            return View(model);
        }
        //เมื่อกดปุ่ม Create
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var model = await service.GetCreateFormAsync(ct); //เรียก Service เพื่อเตรียมข้อมูลสำหรับฟอร์ม
            return View(model); //แล้วส่งข้อมูล (Model) ไปแสดงผลที่หน้าจอ (View: Create.cshtml)
        }

        //เมื่อกดปุ่ม save ข้อมูลทั้งหมดจะถูกส่งมาที่นี่
        //รับข้อมูลจากหน้าจอ แล้วบันทึกลง Database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleFormModel model, CancellationToken ct)
        {
            if (ModelState.IsValid)  // เช็คว่ากรอกข้อมูลครบตามเงื่อนไขไหม (เช่น ชื่อห้ามว่าง)
            {
                bool isSuccess = await service.CreateRoleAsync(model, GetCurrentUserId(), ct); // เรียก Service เพื่อบันทึกข้อมูล ง model (ข้อมูล) และ UserId (คนทำรายการ) ไปให้

                if (!isSuccess) //ถ้ามีชื่อนี้แล้ว 
                {
                    ModelState.AddModelError("RoleName", "Role Name already exists.");
                    return View(model); // กลับไปหน้า Create ให้แก้ชื่อ
                }

                return RedirectToAction(nameof(Index)); // ถ้าสำเร็จให้เด้งไปหน้า Index (แสดงรายการ Role Manage)
            }
            return View(model);
        }

        //เมื่อกดปุ่ม Edit
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var model = await service.GetEditFormAsync(id, ct); // เรียก Service ให้ไปดึงข้อมูลของ ID นี้มาหน่อย
            if (model == null) return NotFound(); //ถ้าหา RoleId ไม่เจอ
            return View(model); //ถ้าหาเจอส่ง Model ที่มีค่ามาให้หน้า Edit
        }

        //เมื่อกด Save Changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleFormModel model, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                bool isSuccess = await service.UpdateRoleAsync(model, GetCurrentUserId(), ct); // ส่งข้อมูลไปอัปเดต (ส่ง User ID ไปด้วยเพื่อเก็บ Log ว่าใครแก้)

                if (!isSuccess)
                {
                    ModelState.AddModelError("RoleName", "Role Name already exists."); // ถ้าชื่อซ้ำ แจ้งผู้ใช้
                    return View(model); //เด้งกลับหน้า Edit เพื่อกรอก Role Name ใหม่
                }

                return RedirectToAction(nameof(Index)); //ถ้าสำเร็จเด้งไปหน้าแสดงรายการของ Role Manage
            }
            return View(model);
        }

        //เมื่อยืนยันการลบ คำสั่งจะถูกส่งมาที่นี่
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await service.DeleteRoleAsync(id, GetCurrentUserId(), ct); // เรียก Service ให้จัดการลบ (ส่ง UserID ไปด้วย เพื่อบันทึกว่าใครเป็นคนลบ)
            return RedirectToAction(nameof(Index)); // ลบเสร็จ เด้งไปหน้าแสดงรายการ Role Manage (index)
        }
    }
}
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Models;

namespace WorkShop.Web.Controllers
{
    //Authorize Policy "CanManageRoles" (AD002)
    [Authorize(Policy = "CanManageRoles")]
    public class RoleManageController(IRoleManageService service) : Controller
    {
        private int GetCurrentUserId() //ดึง UserId ของคน Login ใช้เพื่อส่งเข้าไปใน Service เวลาที่มีการ สร้าง/แก้ไข/ลบ ข้อมูล เพื่อบันทึกว่า ใครเป็นคนทำรายการนี้
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }

        // แสดงหน้าหลัก (Container) สำหรับ AJAX
        public IActionResult Index()
        {
            return View(); // คืนค่า View หลักเปล่าๆ ที่มี JavaScript รอโหลดข้อมูล
        }

        // Action ใหม่: สำหรับโหลดข้อมูลตารางและแบ่งหน้า (Pagination) ผ่าน AJAX
        [HttpGet]
        public async Task<IActionResult> GetRoleList(string? keyword, int pageNo = 1, CancellationToken ct = default)
        {
            // สร้าง Model สำหรับส่งไป Service
            var searchModel = new RoleSearchModel
            {
                Keyword = keyword,
                PageNo = pageNo,
                PageSize = 5
            };

            // เรียก Service เพื่อค้นหาและแบ่งหน้า (Pagination)
            var model = await service.SearchRolesAsync(searchModel, ct);

            // ส่งกลับเป็น Partial View (_RoleTable) เพื่อนำไปแปะในหน้า Index
            return PartialView("_RoleTable", model);
        }

        //เมื่อกดปุ่ม Create
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var model = await service.GetCreateFormAsync(ct); //เรียก Service เพื่อเตรียมข้อมูลสำหรับฟอร์ม
            return View(model); //แล้วส่งข้อมูล (Model) ไปแสดงผลที่หน้าจอ (View: Create.cshtml)
        }

        //เมื่อกดปุ่ม save ข้อมูลทั้งหมดจะถูกส่งมาที่นี่
        //รับข้อมูลจากหน้าจอ แล้วบันทึกลง Database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleFormModel model, CancellationToken ct)
        {
            if (ModelState.IsValid)  // เช็คว่ากรอกข้อมูลครบตามเงื่อนไขไหม (เช่น ชื่อห้ามว่าง)
            {
                bool isSuccess = await service.CreateRoleAsync(model, GetCurrentUserId(), ct); // เรียก Service เพื่อบันทึกข้อมูล ง model (ข้อมูล) และ UserId (คนทำรายการ) ไปให้

                if (!isSuccess) //ถ้ามีชื่อนี้แล้ว 
                {
                    ModelState.AddModelError("RoleName", "Role Name already exists.");
                    return View(model); // กลับไปหน้า Create ให้แก้ชื่อ
                }

                return RedirectToAction(nameof(Index)); // ถ้าสำเร็จให้เด้งไปหน้า Index (แสดงรายการ Role Manage)
            }
            return View(model);
        }

        //เมื่อกดปุ่ม Edit
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var model = await service.GetEditFormAsync(id, ct); // เรียก Service ให้ไปดึงข้อมูลของ ID นี้มาหน่อย
            if (model == null) return NotFound(); //ถ้าหา RoleId ไม่เจอ
            return View(model); //ถ้าหาเจอส่ง Model ที่มีค่ามาให้หน้า Edit
        }

        //เมื่อกด Save Changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleFormModel model, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                bool isSuccess = await service.UpdateRoleAsync(model, GetCurrentUserId(), ct); // ส่งข้อมูลไปอัปเดต (ส่ง User ID ไปด้วยเพื่อเก็บ Log ว่าใครแก้)

                if (!isSuccess)
                {
                    ModelState.AddModelError("RoleName", "Role Name already exists."); // ถ้าชื่อซ้ำ แจ้งผู้ใช้
                    return View(model); //เด้งกลับหน้า Edit เพื่อกรอก Role Name ใหม่
                }

                return RedirectToAction(nameof(Index)); //ถ้าสำเร็จเด้งไปหน้าแสดงรายการของ Role Manage
            }
            return View(model);
        }

        //เมื่อยืนยันการลบ คำสั่งจะถูกส่งมาที่นี่ (ปรับเป็น Return JSON สำหรับ AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await service.DeleteRoleAsync(id, GetCurrentUserId(), ct); // เรียก Service ให้จัดการลบ

                // คืนค่า JSON แจ้ง Client ว่าลบสำเร็จ
                return Json(new { success = true, message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                // คืนค่า JSON แจ้ง Client ว่าเกิดข้อผิดพลาด
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}