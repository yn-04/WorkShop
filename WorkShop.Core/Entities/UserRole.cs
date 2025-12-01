using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkShop.Core.Entities
{
    public class UserRole
    {
        [Key, Column(Order = 0)] 
        public long UserId { get; set; } 

        [Key, Column(Order = 1)]
        public int RoleId { get; set; }

        public DateTime CreatedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}