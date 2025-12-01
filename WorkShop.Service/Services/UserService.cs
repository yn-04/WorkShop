using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkShop.Core.Entities;
using WorkShop.Core.Interfaces;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace WorkShop.Service.Services
{
    public class UserService(IUserRepository users, IRepository<User> repo) : IUserService
    {
        private readonly PasswordHelper _passwordHelper = new PasswordHelper();

        public async Task<User?> LoginhAsync(string email, string password, CancellationToken ct = default)
        {
            //ใช้ users (IUserRepository) เพื่อดึง Role มาด้วย
            var existing = await users.GetByEmailAsync(email, ct);

            if (existing is null)
            {
                return null;
            }

            if (!_passwordHelper.VerifyPassword(existing.Password, password))
            {
                return null; // รหัสผ่านไม่ถูกต้อง
            }

            return existing;
        }

        public async Task<User?> GetUserByIdAsync(int id, CancellationToken ct = default)
        {
            var user = await repo.GetByIdAsync(id, ct);

            return user; // คืนค่า user ที่ค้นหาเจอ (หรือ null ถ้าไม่เจอ)
        }
    }
}