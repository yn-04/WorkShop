using System;

namespace WorkShop.Service.Models
{
    public class SearchBaseModel
    {
        public string? Keyword { get; set; }

        // Pagination Properties
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 5; // ✅ จำกัด 5 รายการต่อหน้า
        public int TotalRecords { get; set; }

        // คำนวณจำนวนหน้าทั้งหมด
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        public bool HasPreviousPage => PageNo > 1;
        public bool HasNextPage => PageNo < TotalPages;
    }
}