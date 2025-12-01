using System.Security.Claims;
using WorkShop.Web.Models;

namespace WorkShop.Service.Interfaces
{
    public interface IMenuService
    {
        // รับ User (ClaimsPrincipal) เพื่อไปหาว่าคือใครและมีสิทธิ์อะไร
        Task<List<MenuViewModel>> GetAllowedMenusAsync(ClaimsPrincipal user);
    }
}