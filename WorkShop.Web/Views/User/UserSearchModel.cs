using WorkShop.Core.Entities;

namespace WorkShop.Web.Model
{
    public class UserSearchModel
    {
        // คำค้นหา
        public string? Email { get; set; }
        public string? Phone { get; set; } // เพิ่ม Phone
        public string? DisplayName { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; } // null=All, true=Active, false=Inactive

        // ผลลัพธ์
        public List<User> Users { get; set; } = new List<User>();

        // ข้อมูลสำหรับ Dropdown
        public List<Role> AvailableRoles { get; set; } = new List<Role>();
    }
}