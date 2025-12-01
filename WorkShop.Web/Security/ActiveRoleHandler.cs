using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorkShop.Infrastructure.Data; // เรียกใช้ DbContext

namespace WorkShop.Web.Security // ✅ ใช้ Namespace เป็น Security
{
    // 1. สร้าง "ข้อกำหนด" (Requirement)
    public class ActiveRoleRequirement : IAuthorizationRequirement
    {
    }

    // 2. สร้าง "ผู้ตรวจสอบ" (Handler)
    public class ActiveRoleHandler : AuthorizationHandler<ActiveRoleRequirement>
    {
        private readonly WorkShopDbContext _context;

        public ActiveRoleHandler(WorkShopDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveRoleRequirement requirement)
        {
            // 1. เช็คว่า Login หรือยัง
            var user = context.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return;
            }

            // 2. ดึง UserId จาก Claims (ClaimTypes.NameIdentifier)
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return;
            }

            // 3. เช็ค Real-time จาก Database
            // Logic: ต้องมี Role ที่ Active (IsActive == 1) อย่างน้อย 1 อันที่ผูกกับ User นี้
            var hasActiveRole = await _context.UserRoles
                .Include(ur => ur.Role)
                .AnyAsync(ur => ur.UserId == userId && ur.Role.IsActive == 1); // ✅ เช็ค == 1 ตาม Type byte ใน DB

            if (hasActiveRole)
            {
                context.Succeed(requirement); // ✅ ผ่าน: มี Role ที่ใช้งานได้
            }
            else
            {
                context.Fail(); // ❌ ไม่ผ่าน: Role ทั้งหมดเป็น Inactive หรือไม่มี Role เลย
            }
        }
    }
}