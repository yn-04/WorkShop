using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations;

namespace WorkShop.Service.Models
{
    public class UserFormModel //ใช้ตอนสร้างและแก้ไข
    {
        public long UserId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Display Name is required")]
        public string DisplayName { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }

        public int IsActive { get; set; } = 1;

        [Required(ErrorMessage = "Please select at least one role")]
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}