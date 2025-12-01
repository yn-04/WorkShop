namespace WorkShop.Web.Models
{
    public class MenuViewModel
    {
        public string MenuName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string IconClass { get; set; } // ใช้แสดง icon ใน View
        public int SortOrder { get; set; }
    }
}