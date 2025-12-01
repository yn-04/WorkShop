using WorkShop.Service.Models;

namespace WorkShop.Service.Interfaces
{
    public interface IRoleManageService
    {
        Task<RoleSearchModel> SearchRolesAsync(RoleSearchModel model, CancellationToken ct = default);
        Task<List<RoleViewModel>> GetAllRolesAsync(string? keyword = null, CancellationToken ct = default);

        Task<RoleFormModel> GetCreateFormAsync(CancellationToken ct = default);

        Task<bool> CreateRoleAsync(RoleFormModel model, int userId, CancellationToken ct = default);

        Task<RoleFormModel> GetEditFormAsync(int id, CancellationToken ct = default);

        Task<bool> UpdateRoleAsync(RoleFormModel model, int userId, CancellationToken ct = default);

        Task DeleteRoleAsync(int id, int userId, CancellationToken ct = default);
    }
}