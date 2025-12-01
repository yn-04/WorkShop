using System.ComponentModel.DataAnnotations;

namespace WorkShop.Service.Models
{
    // สำหรับแสดงในตารางหน้า Index
    public class RoleViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    // สำหรับหน้า Create และ Edit
    public class RoleFormModel
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Please enter Role Name")]
        public string RoleName { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // รายการเมนูสำหรับทำ Checkbox
        public List<MenuSelectionViewModel> MenuList { get; set; } = new List<MenuSelectionViewModel>();
    }

    public class MenuSelectionViewModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public bool IsSelected { get; set; } // ค่าที่ถูกติ๊ก
        public string? Description { get; set; }
    }
}