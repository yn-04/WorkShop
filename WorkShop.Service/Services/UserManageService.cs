using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkShop.Core.Entities;
using WorkShop.Infrastructure.Data;
using WorkShop.Service.Helpers;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Models;

namespace WorkShop.Service.Services
{
    public class UserManageService(WorkShopDbContext db) : IUserManageService
    {
        public async Task<UserSearchModel> SearchUsersAsync(UserSearchModel model, CancellationToken ct = default)
        {
            var query = db.Users
                .Include(u => u.UserRoles) //Join ตาราง UserRoles
                .ThenInclude(ur => ur.Role) // Join ต่อไปหาตาราง Role
                .AsQueryable(); // ยังไม่ยิง Database นะ เตรียมคำสั่งไว้ก่อน

            query = query.Where(u => u.IsDelete == 0); // กรองเอาเฉพาะ User ที่ยังไม่ถูกลบ

            if (!string.IsNullOrEmpty(model.Email))
                query = query.Where(u => u.Email.Contains(model.Email)); // กรอง Email Contains คือ ค้นหาบางส่วนได้

            if (!string.IsNullOrEmpty(model.Phone))
                query = query.Where(u => u.Phone.Contains(model.Phone)); // กรอง Phone Contains คือ ค้นหาบางส่วนได้

            if (!string.IsNullOrEmpty(model.DisplayName))
            {
                var terms = model.DisplayName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries); // ตัดคำด้วยช่องว่าง
                foreach (var term in terms)
                {
                    query = query.Where(u => u.DisplayName.Contains(term));
                }
            }

            if (model.IsActive.HasValue)
            {
                byte activeVal = (byte)model.IsActive.Value;
                query = query.Where(u => u.IsActive == activeVal); // กรองสถานะ Active/Inactive
            }

            if (model.RoleId.HasValue)
                query = query.Where(u => u.UserRoles.Any(r => r.RoleId == model.RoleId.Value)); //ค้นหา User ที่มี Role ตรงกับที่เลือก

            // นับจำนวนทั้งหมดก่อน 
            model.TotalRecords = await query.CountAsync(ct);

            // คำนวณ Pagination 
            // ถ้า PageSize เป็น 0 ให้ตั้งค่า Default เป็น 5 (ป้องกัน Error หารด้วยศูนย์)
            if (model.PageSize <= 0) model.PageSize = 5;
            if (model.PageNo <= 0) model.PageNo = 1;

            //ดึงข้อมูลจริง (Fetch Data)
            var users = await query
                .OrderByDescending(u => u.CreatedDate) // เรียงคนใหม่ล่าสุดขึ้นก่อน
                .Skip((model.PageNo - 1) * model.PageSize) // ข้ามหน้าก่อนหน้า
                .Take(model.PageSize)                      // เอามาแค่ 5 คน
                .Select(u => new UserViewModel 
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    CreatedDate = u.CreatedDate,
                    StatusName = u.IsActive == 1 ? "Active" : "Inactive", 
                    RoleNames = string.Join(", ", u.UserRoles.Select(ur => ur.Role.RoleName))
                })
                .ToListAsync(ct); // ยิงคำสั่ง SQL ไปที่ Database

            model.Results = users;
            return model;
        }

        public async Task<List<Role>> GetAllRolesAsync(CancellationToken ct = default)
        {
            // ✅ FIX: IsActive เป็น byte ต้องเทียบกับ 1
            return await db.Roles.Where(r => r.IsActive == 1).ToListAsync(ct);
        }

        public async Task CreateUserAsync(UserFormModel model, long currentUserId, CancellationToken ct = default)
        {
            var passwordHelper = new PasswordHelper();
            string pwdToHash = model.Password ?? string.Empty;

            var newUser = new User
            {
                Email = model.Email,
                Password = passwordHelper.HashPassword(pwdToHash),
                DisplayName = model.DisplayName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                IsActive = (byte)model.IsActive,
                CreatedDate = DateTime.Now,
                IsDelete = 0,
                CreatedBy = currentUserId
            };

            db.Users.Add(newUser);
            await db.SaveChangesAsync(ct);

            if (model.RoleIds != null && model.RoleIds.Any())
            {
                foreach (var roleId in model.RoleIds)
                {
                    db.UserRoles.Add(new UserRole
                    {
                        UserId = newUser.UserId,
                        RoleId = roleId,
                        CreatedDate = DateTime.Now
                    });
                }
                await db.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string email, long? excludeUserId = null, CancellationToken ct = default)
        {
            var query = db.Users.Where(u => u.Email == email && u.IsDelete == 0);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.UserId != excludeUserId.Value);
            }

            return await query.AnyAsync(ct);
        }

        public async Task<UserFormModel?> GetUserForEditAsync(long id, CancellationToken ct = default)
        {
            var user = await db.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == id, ct);

            if (user == null) return null;

            return new UserFormModel
            {
                UserId = user.UserId,
                Email = user.Email,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                IsActive = (int)user.IsActive, // byte to int
                RoleIds = user.UserRoles.Select(r => r.RoleId).ToList()
            };
        }

        public async Task UpdateUserAsync(UserFormModel model, long currentUserId, CancellationToken ct = default)
        {
            var user = await db.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == model.UserId, ct);

            if (user == null) return;

            user.Email = model.Email;
            user.DisplayName = model.DisplayName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;
            user.IsActive = (byte)model.IsActive;
            user.UpdatedDate = DateTime.Now;
            user.UpdatedBy = currentUserId;

            if (!string.IsNullOrEmpty(model.Password))
            {
                var passwordHelper = new PasswordHelper();
                user.Password = passwordHelper.HashPassword(model.Password);
            }

            var currentRoles = user.UserRoles.ToList();
            if (currentRoles.Any())
            {
                db.UserRoles.RemoveRange(currentRoles);
            }

            if (model.RoleIds != null && model.RoleIds.Any())
            {
                foreach (var roleId in model.RoleIds)
                {
                    db.UserRoles.Add(new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = roleId,
                        CreatedDate = DateTime.Now
                    });
                }
            }
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteUserAsync(long id, CancellationToken ct = default)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserId == id, ct);

            if (user != null)
            {
                user.IsDelete = 1;
                user.UpdatedDate = DateTime.Now;

                await db.SaveChangesAsync(ct);
            }
        }
    }
}