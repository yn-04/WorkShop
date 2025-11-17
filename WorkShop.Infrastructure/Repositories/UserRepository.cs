using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using WorkShop.Core.Entities;
using WorkShop.Core.Interfaces;
using WorkShop.Infrastructure.Data;

namespace WorkShop.Infrastructure.Repositories
{
    public class UserRepository(WorkShopDbContext db) : EfRepository<User>(db), IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) //ค้นหาผู้ใช้ในฐานข้อมูลด้วยอีเมล
            => db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct); //มันจะวิ่งไปที่ตาราง Users และค้นหาแถว (Row) ที่คอลัมน์ Email ตรงกับอีเมลที่ส่งเข้ามา ถ้าเจอ: ก็ส่งข้อมูลผู้ใช้กลับไป ถ้าไม่เจอ: ก็ส่งค่า null กลับไป
            //AsNoTracking อ่านค่าได้อย่างเดียว แก้ไขไม่ได้ ตอนนี้ต้องการอ่านข้อมูลมา "ตรวจสอบ" รหัสผ่าน ไม่ได้จะแก้ไขข้อมูลจึงเลือกใช้อันนี้ไปก่อน    
    }
}
