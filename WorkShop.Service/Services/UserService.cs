using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkShop.Core.Entities;
using WorkShop.Core.Interfaces;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Helpers; // เพิ่ม using สำหรับ Helper
using static System.Net.Mime.MediaTypeNames;

namespace WorkShop.Service.Services
{
    public class UserService(IUserRepository users, IRepository<User> repo) : IUserService 
    {
        // สร้าง instance ของ PasswordHelper
        private readonly PasswordHelper _passwordHelper = new PasswordHelper();

        public async Task<User?> LoginhAsync(string email, string password, CancellationToken ct = default) //ตรวจสอบการล็อกอินทั้งหมดตั้งแต่ต้นจนจบ
        {
            var existing = await users.GetByEmailAsync(email, ct); //ค้นหาผู้ใช้ด้วยอีเมลที่ถูกส่งมา

            // ตรวจสอบว่ามีผู้ใช้นี้หรือไม่
            if (existing is null)
            {
                return null; // ไม่พบผู้ใช้ อีเมลผิด
            }

            // ตรวจสอบรหัสผ่าน
            // ถ้าหาอีเมลเจอจะตรวจสอบรหัสผ่าน ถ้ารหัสผ่านที่ผู้ใช้กรอกกับรหัสผ่านในฐานข้อมูลตรงกันก็จะล้อกอินได้
            if (!_passwordHelper.VerifyPassword(existing.Password, password))
            {
                return null; // รหัสผ่านไม่ถูกต้อง
            }

            // ถ้าผ่านทั้งหมด = ล็อกอินสำเร็จ
            return existing;
        }
    }
}