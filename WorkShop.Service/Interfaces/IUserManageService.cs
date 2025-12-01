using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkShop.Core.Entities;
using WorkShop.Service.Models;

namespace WorkShop.Service.Interfaces
{
    public interface IUserManageService
    {
        Task<UserSearchModel> SearchUsersAsync(UserSearchModel model, CancellationToken ct = default);
        Task<List<Role>> GetAllRolesAsync(CancellationToken ct = default);
        Task CreateUserAsync(UserFormModel model, long currentUserId, CancellationToken ct = default);
        Task<UserFormModel?> GetUserForEditAsync(long id, CancellationToken ct = default);
        Task UpdateUserAsync(UserFormModel model, long currentUserId, CancellationToken ct = default);
        Task<bool> CheckEmailExistsAsync(string email, long? excludeUserId = null, CancellationToken ct = default);
        Task DeleteUserAsync(long id, CancellationToken ct = default);
    }
}