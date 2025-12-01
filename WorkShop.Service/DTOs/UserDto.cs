namespace WorkShop.Service.DTOs
{
    //กล่องสำหรับส่งข้อมูลผู้ใช้
    public class UserDto
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
    }
}