using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkShop.Core.Entities
{
    public partial class Role
    {
        public Role()
        {
            RoleMenuPermissions = new HashSet<RoleMenuPermission>();
            UserRoles = new HashSet<UserRole>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        [Column("Decription")]
        public string? Description { get; set; }
        public byte IsActive { get; set; }

        // --- เพิ่มใหม่ ---
        public byte IsDelete { get; set; } // tinyint -> byte (0=ปกติ, 1=ลบ)
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; } // bigint -> long? (Nullable เผื่อไว้)
        // ----------------

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}