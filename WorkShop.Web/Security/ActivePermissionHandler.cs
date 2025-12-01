using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorkShop.Infrastructure.Data;

namespace WorkShop.Web.Security
{
    public class ActivePermissionHandler : AuthorizationHandler<ActivePermissionRequirement>
    {
        private readonly WorkShopDbContext _context;

        public ActivePermissionHandler(WorkShopDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ActivePermissionRequirement requirement)
        {
            // 1. เช็ค Login
            var user = context.User;
            if (user == null || !user.Identity.IsAuthenticated) return;

            // 2. ดึง UserId
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId)) return;

            // 3. Logic หัวใจสำคัญ:
            // "ค้นหาว่า User นี้มี Role ไหนบ้างที่..."
            //  - เป็น Active (IsActive == 1)
            //  - และ Role นั้นผูกกับ Menu ที่มี MenuCode ตรงกับที่ต้องการ (เช่น "AD002")

            var hasActivePermission = await _context.UserRoles
                .AnyAsync(ur =>
                    ur.UserId == userId &&                 // เป็นของ User คนนี้
                    ur.Role.IsActive == 1 &&               // Role ต้อง Active
                    ur.Role.RoleMenuPermissions.Any(rmp => // Role นี้ต้องมีสิทธิ์ในเมนูนี้
                        rmp.Menu.MenuCode == requirement.RequiredPermissionCode &&
                        rmp.Menu.IsActive == 1             // (แถม) ตัวเมนูเองก็ต้อง Active
                    )
                );

            if (hasActivePermission)
            {
                context.Succeed(requirement); // ✅ ผ่าน: มี Role Active ที่ถือสิทธิ์นี้อยู่จริง
            }
            else
            {
                context.Fail(); // ❌ ไม่ผ่าน: Role ที่ถือสิทธิ์นี้ Inactive หรือไม่มีสิทธิ์นี้เลย
            }
        }
    }
}