using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkShop.Core.Entities
{
    public partial class Menu
    {
        public Menu()
        {
            InverseParentMenu = new HashSet<Menu>();
            RoleMenuPermissions = new HashSet<RoleMenuPermission>();
        }

        public int MenuId { get; set; }
        public string MenuCode { get; set; }
        public string MenuName { get; set; }
        public int? ParentMenuId { get; set; }
        public int SortOrder { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string? IconClass { get; set; }
        public byte IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; } // ตรงกับ DbContext แล้ว

        public virtual Menu ParentMenu { get; set; }
        public virtual ICollection<Menu> InverseParentMenu { get; set; }
        public virtual ICollection<RoleMenuPermission> RoleMenuPermissions { get; set; }
    }
}