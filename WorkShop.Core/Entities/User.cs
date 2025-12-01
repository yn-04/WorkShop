using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkShop.Core.Entities
{
    public class User
    {
        [Key]
        public long UserId { get; set; } 

        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }

        public byte IsActive { get; set; }
        public byte IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
    }
}