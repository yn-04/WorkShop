using Microsoft.AspNetCore.Identity;

namespace WorkShop.Service.Helpers
{
    public class PasswordHelper //จัดการการเข้ารหัสรหัสผ่าน และ การตรวจสอบรหัสผ่าน
    {
        private readonly PasswordHasher<string> _passwordHasher;

        public PasswordHelper()
        {
            _passwordHasher = new PasswordHasher<string>();
        }

        public string HashPassword(string password) //เมื่อมีคนสมัครสมาชิกหรือเปลี่ยนรหัสผ่าน เมธอดนี้จะรับรหัสผ่านธรรมดา (เช่น "123456") และแปลงมันให้อยู่ในรูปแบบที่เข้ารหัสแล้ว (เช่น AQAAAAEAACcQ...) เพื่อเก็บลงฐานข้อมูล
        {
            return _passwordHasher.HashPassword(null, password); //คืนค่ารหัสผ่านที่ ถูกแฮชแล้ว
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword) //"ตรวจสอบ" รหัสผ่าน (นี่คือส่วนที่ใช้ตอนล็อกอิน)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword); //hashedPassword: รหัสที่เข้ารหัสไว้ (ดึงมาจากฐานข้อมูล)
                                                                                                       //providedPassword: รหัสผ่านธรรมดา (ที่ผู้ใช้เพิ่งกรอกในฟอร์มล็อกอิน)
            return result == PasswordVerificationResult.Success; 
        }
    }
}