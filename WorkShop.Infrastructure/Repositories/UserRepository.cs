using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using WorkShop.Core.Entities;
using WorkShop.Core.Interfaces;
using WorkShop.Infrastructure.Data;

namespace WorkShop.Infrastructure.Repositories
{
    public class UserRepository(WorkShopDbContext db) : EfRepository<User>(db), IUserRepository
    {
        // นี่คือเมธอดที่ถูกแก้ไข
        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return db.Users
                .Include(u => u.UserRoles)
                .AsNoTracking()
                // ✅✅✅ แก้ไขตรงนี้ครับ ✅✅✅
                // เพิ่มเงื่อนไข: ต้องเป็นอีเมลนี้ AND (ต้องยังไม่ลบ OR ค่าเป็น null) AND (ต้อง Active)
                .FirstOrDefaultAsync(u =>
                    u.Email == email &&
                    (u.IsDelete == 0 || u.IsDelete == null) &&
                    u.IsActive == 1
                , ct);
        }
    }
}