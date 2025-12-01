using System;
using System.Collections.Generic;

namespace WorkShop.Service.Models
{
    // ✅ สืบทอดจาก SearchBaseModel เพื่อรองรับ Pagination
    public class UserSearchModel : SearchBaseModel
    {
        // Filter เฉพาะ User
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? DisplayName { get; set; }
        public int? RoleId { get; set; }
        public int? IsActive { get; set; } // ในหน้าจอเป็น Dropdown 

        // ผลลัพธ์
        public List<UserViewModel> Results { get; set; } = new List<UserViewModel>();
    }

    // ส่งคืนค่าที่แสดงผลในหน้าจอ
    public class UserViewModel
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RoleNames { get; set; }
    }
}