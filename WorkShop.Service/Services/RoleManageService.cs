using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkShop.Core.Entities;
using WorkShop.Infrastructure.Data;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Models; 
namespace WorkShop.Service.Services
{
    public class RoleManageService(WorkShopDbContext db) : IRoleManageService
    {
        // Method สำหรับดึงข้อมูลแบบ Search + Pagination (ใช้ในหน้า Index)
        public async Task<RoleSearchModel> SearchRolesAsync(RoleSearchModel model, CancellationToken ct = default)
        {
            // Query เริ่มต้น (เฉพาะที่ไม่ถูกลบ)
            var query = db.Roles.Where(r => r.IsDelete == 0);

            // 2. กรองด้วย Keyword (ถ้ามี)
            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                query = query.Where(r => r.RoleName.Contains(model.Keyword));
            }

            // นับจำนวนทั้งหมดก่อนแบ่งหน้า (TotalRecords)
            model.TotalRecords = await query.CountAsync(ct);

            // 4. ตั้งค่า Default Pagination
            if (model.PageSize <= 0) model.PageSize = 5; //  บังคับ 5 รายการ
            if (model.PageNo <= 0) model.PageNo = 1;

            //  ดึงข้อมูลตามหน้า (Skip & Take)
            var roles = await query
                .Select(r => new RoleViewModel
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    IsActive = r.IsActive == 1, 
                    CreatedDate = r.CreatedDate
                })
                .OrderByDescending(r => r.RoleId)
                .Skip((model.PageNo - 1) * model.PageSize) // ข้ามหน้าก่อนหน้า
                .Take(model.PageSize)                      // เอาแค่ 5 แถว
                .ToListAsync(ct);

            model.Results = roles;
            return model;
        }

        //  Method เดิม (ถ้ายังจำเป็นต้องใช้ในบางจุด เช่น Dropdown)
        public async Task<List<RoleViewModel>> GetAllRolesAsync(string? keyword = null, CancellationToken ct = default)
        {
            var query = db.Roles.Where(r => r.IsDelete == 0);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(r => r.RoleName.Contains(keyword));
            }

            return await query
                .Select(r => new RoleViewModel
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    IsActive = r.IsActive == 1,
                    CreatedDate = r.CreatedDate
                })
                .OrderByDescending(r => r.RoleId)
                .ToListAsync(ct);
        }

        //ดึงข้อมูลเมนูสำหรับหน้า Create
        public async Task<RoleFormModel> GetCreateFormAsync(CancellationToken ct = default)
        {
            var allMenus = await db.Menus
                .Where(m => m.IsActive == 1) 
                .OrderBy(m => m.SortOrder)
                .ToListAsync(ct);

            return new RoleFormModel
            {
                IsActive = true,
                MenuList = allMenus.Select(m => new MenuSelectionViewModel
                {
                    MenuId = m.MenuId,
                    MenuName = m.MenuName,
                    IsSelected = false
                }).ToList()
            };
        }

        // สร้าง Role ใหม่
        public async Task<bool> CreateRoleAsync(RoleFormModel model, int userId, CancellationToken ct = default)
        {
            // เช็คชื่อซ้ำ 
            bool isExists = await db.Roles.AnyAsync(r => r.RoleName == model.RoleName && r.IsDelete == 0, ct);
            if (isExists) return false;

            // สร้าง Role เพื่อบันทึกลง Database
            var role = new Role
            {
                RoleName = model.RoleName,
                Description = model.Description,
                IsActive = model.IsActive ? (byte)1 : (byte)0, 
                IsDelete = 0,
                CreatedBy = userId,
                UpdatedBy = null,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            db.Roles.Add(role);
            await db.SaveChangesAsync(ct);

            // บันทึกสิทธิ์ (Permission): วนลูปเฉพาะอันที่ติ๊กถูก (IsSelected)
            if (model.MenuList != null && model.MenuList.Any(m => m.IsSelected))
            {
                var permissions = model.MenuList
                    .Where(m => m.IsSelected)
                    .Select(m => new RoleMenuPermission
                    {
                        RoleId = role.RoleId,
                        MenuId = m.MenuId,
                        Permission = 1,
                        CreatedDate = DateTime.Now
                    });

                await db.RoleMenuPermissions.AddRangeAsync(permissions, ct);
                await db.SaveChangesAsync(ct);
            }

            return true;
        }

        // ดึงข้อมูล Role มาแสดงในหน้า Edit
        public async Task<RoleFormModel> GetEditFormAsync(int id, CancellationToken ct = default)
        {
            var role = await db.Roles.FindAsync(new object[] { id }, ct); //ดึงข้อมูล Role Name, Description
            if (role == null || role.IsDelete == 1) return null;

            var existingPerms = await db.RoleMenuPermissions //ดึงสิทธิ์ (Permission) ที่ Role นี้มีอยู่แล้ว ออกมาเป็น List ของ MenuId
                .Where(rm => rm.RoleId == id)
                .Select(rm => rm.MenuId)
                .ToListAsync(ct);

            var allMenus = await db.Menus //ดึงเมนูทั้งหมดที่ Active
                .Where(m => m.IsActive == 1)
                .OrderBy(m => m.SortOrder)
                .ToListAsync(ct);

            return new RoleFormModel //วนลูปเมนูทั้งหมด แล้วเช็คว่าเมนูนี้ผู้ใช้เคยเลือกไว้ไหม
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description,
                IsActive = role.IsActive == 1, 
                MenuList = allMenus.Select(m => new MenuSelectionViewModel
                {
                    MenuId = m.MenuId,
                    MenuName = m.MenuName,
                    IsSelected = existingPerms.Contains(m.MenuId)
                }).ToList()
            };
        }

        // อัปเดตข้อมูล Role
        public async Task<bool> UpdateRoleAsync(RoleFormModel model, int userId, CancellationToken ct = default)
        {
            //เช็คชื่อซ้ำ (ซ้ำกับคนอื่นได้ แต่ห้ามซ้ำกับตัวเอง)
            bool isExists = await db.Roles.AnyAsync(r => r.RoleName == model.RoleName && r.IsDelete == 0&& r.RoleId != model.RoleId, ct);

            if (isExists) return false;

            //อัปเดตข้อมูลลงตาราง Role
            var role = await db.Roles.FindAsync(new object[] { model.RoleId }, ct);
            if (role == null) return false;
            role.RoleName = model.RoleName;
            role.Description = model.Description;
            role.IsActive = model.IsActive ? (byte)1 : (byte)0;
            role.UpdatedBy = userId;
            role.UpdatedDate = DateTime.Now;

            //ลบสิทธิ์เก่า 
            var oldPerms = await db.RoleMenuPermissions
                                   .Where(x => x.RoleId == model.RoleId)
                                   .ToListAsync(ct);

            if (oldPerms.Any())
            {
                db.RoleMenuPermissions.RemoveRange(oldPerms);
            }

            //เพิ่มสิทธิ์ใหม่
            if (model.MenuList != null && model.MenuList.Any(m => m.IsSelected))
            {
                var newPermissions = model.MenuList
                    .Where(m => m.IsSelected)
                    .Select(m => new RoleMenuPermission
                    {
                        RoleId = model.RoleId,
                        MenuId = m.MenuId,
                        Permission = 1, 
                        CreatedDate = DateTime.Now
                    });

                await db.RoleMenuPermissions.AddRangeAsync(newPermissions, ct);
            }

            await db.SaveChangesAsync(ct); 

            return true;
        }

        // อัปเดตสถานะ Role ใน Database
        public async Task DeleteRoleAsync(int id, int userId, CancellationToken ct = default)
        {
            var role = await db.Roles.FindAsync(new object[] { id }, ct); //หาข้อมูล Role นั้นมาก่อน
            if (role != null)
            {
                role.IsDelete = 1;
                role.UpdatedBy = userId;
                role.UpdatedDate = DateTime.Now;
                await db.SaveChangesAsync(ct);
            }
        }
    }
}