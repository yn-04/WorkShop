using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkShop.Core.Entities;

namespace WorkShop.Service.Interfaces
{
    public interface IUserService
    {
        // แก้ไขตรงนี้: เพิ่ม ? ให้เป็น Task<User?>
        Task<User?> LoginhAsync(string email, string password, CancellationToken ct = default);
    }
}